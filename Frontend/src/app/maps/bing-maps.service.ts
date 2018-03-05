// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/// <reference path='../../../node_modules/bingmaps/types/MicrosoftMaps/Microsoft.Maps.All.d.ts' />

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

import { Asset, AssetType } from '../assets/asset';
import { Point } from '../shared/point';
import { TrackingPoint } from '../shared/tracking-point';
import { Geofence, AreaType } from '../shared/geofence';
import { Trip } from '../shared/trip';
import { Location } from '../shared/location';
import { TripService } from '../core/trip.service';
import { LocationService } from '../locations/location.service';

import 'rxjs/add/operator/map';

@Injectable()
export class BingMapsService {
    static colors = ['#56A6B6', '#464E50', '#56B691', '#BF8B8A', '#E95794'];
    static tripColors = ['#000CCC', '#CC0200', '#73CC15', '#00EAFF'];
    private loadPromise: Promise<void>;
    private map: Microsoft.Maps.Map;
    private infobox: Microsoft.Maps.Infobox;
    private searchManager: Microsoft.Maps.Search.SearchManager;
    private trip: Trip;
    private readonly yellowSpeed = 4.4;
    private readonly greenSpeed = 13.4;
    private drawHandlerId;
    private previousHighlightPoint: Microsoft.Maps.Pushpin;

    constructor(
        private tripService: TripService,
        private locationService: LocationService
    ) { }

    init(element: HTMLElement, options: Microsoft.Maps.IMapLoadOptions): void {
        this.load().then(() => {
            this.map = new Microsoft.Maps.Map(element, options);

            if (!this.searchManager) {
                Microsoft.Maps.loadModule('Microsoft.Maps.Search', () => {
                    this.searchManager = new Microsoft.Maps.Search.SearchManager(this.map);
                });
            }

            Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                if (this.trip) {
                    const event = e as Microsoft.Maps.IMouseEventArgs;
                    this.tripService.getPoints(
                        this.trip.id,
                        event.location.latitude,
                        event.location.longitude
                    ).subscribe(points => {
                        for (const point of points) {
                            let pinColor;
                            if (point.speed >= this.greenSpeed) {
                                pinColor = 'green';
                            } else if (
                                point.speed < this.greenSpeed &&
                                point.speed >= this.yellowSpeed
                            ) {
                                pinColor = 'yellow';
                            } else {
                                pinColor = 'red';
                            }
                            const pp = this.createPushPin(
                                point,
                                this.trip.id,
                                pinColor,
                                this.timeConverter(point.time),
                                `speed: ${Math.round(
                                    point.speed * 3.6
                                )} km/hr, accuracy: ${point.accuracy}`
                            );
                            this.map.entities.add(pp);
                        }
                    });
                }
            });
        });
    }

    showGeofences(geofences: Geofence[]): void {
        this.load().then(() => {
            this.clear();

            const locations = new Array<Microsoft.Maps.Location>();

            // tslint:disable-next-line:forin
            for (const g in geofences) {
                const geofence = geofences[g];

                locations.splice(0, locations.length);
                locations.push.apply(locations,
                    this.drawGeofencePolygon(geofence, 'rgba(86,182,145,0.4)', BingMapsService.colors[0]));
            }

            this.centerMapCentroid(locations);
        });
    }

    showGeofence(geofence: Geofence): void {
        this.load().then(() => {
            this.clear();
            const polygonLocations = this.drawGeofencePolygon(geofence, 'rgba(86,182,145,0.4)', BingMapsService.colors[0]);
            this.centerMapCentroid(polygonLocations);
        });
    }

    drawCircularGeofence(subject: Subject<Point>, initialCenter: Point, initialRadius: number, radius: Observable<number>) {
        this.load().then(() => {
            const currentCenter = initialCenter;
            let currentRadius = initialRadius;
            this.clear();

            if (initialCenter && initialCenter.latitude && initialCenter.longitude && initialRadius) {
                const polygon = new Microsoft.Maps.Polygon(
                    this.getCirclePolygon(currentCenter, currentRadius),
                    {
                        fillColor: 'rgba(86,182,145,0.4)',
                        strokeColor: BingMapsService.colors[0]
                    });

                const pushpin = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(currentCenter.latitude, currentCenter.longitude),
                    {
                        color: BingMapsService.colors[0],
                        icon: '/assets/images/circular-geofence-center-green.svg',
                        anchor: new Microsoft.Maps.Point(12, 12)
                    });

                this.map.entities.push(pushpin);
                this.map.entities.push(polygon);
                this.centerMap([currentCenter]);
            }

            const onCircularGeofenceChanged = () => {
                this.clear();
                const polygon = new Microsoft.Maps.Polygon(
                    this.getCirclePolygon(currentCenter, currentRadius),
                    {
                        fillColor: 'rgba(233,87,148,0.4)',
                        strokeColor: BingMapsService.colors[4]
                    }
                );

                const pushpin = new Microsoft.Maps.Pushpin(
                    new Microsoft.Maps.Location(currentCenter.latitude, currentCenter.longitude),
                    {
                        icon: '/assets/images/circular-geofence-center-pink.svg',
                        anchor: new Microsoft.Maps.Point(12, 12),
                        color: BingMapsService.colors[4]
                    });

                this.map.entities.push(pushpin);
                this.map.entities.push(polygon);

                subject.next(currentCenter);
            };

            this.drawHandlerId = Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                const event = e as Microsoft.Maps.IMouseEventArgs;

                currentCenter.latitude = event.location.latitude;
                currentCenter.longitude = event.location.longitude;

                onCircularGeofenceChanged();
            });
            const currentDrawHandlerId = this.drawHandlerId;
            radius
                .takeWhile(() => currentDrawHandlerId === this.drawHandlerId)
                .subscribe(r => {
                    currentRadius = r;
                    if (currentCenter && currentCenter.latitude && currentCenter.longitude) {
                        onCircularGeofenceChanged();
                    }
                });
        });
    }

    drawPolygonGeofence(subject: Subject<Point[]>, initialPoints: Point[]) {
        this.load().then(() => {
            let polygonPoints = new Array<Point>();
            const locationsArray = new Array<Microsoft.Maps.Location>();
            this.clear();

            if (initialPoints && initialPoints.length > 1) {
                polygonPoints = initialPoints.slice(0);
                initialPoints.forEach(p => {
                    const loc = new Microsoft.Maps.Location(p.latitude, p.longitude);
                    locationsArray.push(loc);
                });

                const polygon = new Microsoft.Maps.Polygon(locationsArray, {
                    fillColor: 'rgba(86,182,145,0.4)',
                    strokeColor: BingMapsService.colors[0]
                });
                this.map.entities.push(polygon);
                this.centerMapCentroid(initialPoints);
            }

            this.drawHandlerId = Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                const event = e as Microsoft.Maps.IMouseEventArgs;
                locationsArray.push(event.location);

                const p = new Point();
                p.latitude = event.location.latitude;
                p.longitude = event.location.longitude;

                polygonPoints.push(p);

                this.clear();
                if (locationsArray.length > 1) {
                    const polygon = new Microsoft.Maps.Polygon(
                        locationsArray.slice(0),
                        {
                            fillColor: 'rgba(233,87,148,0.4)',
                            strokeColor: BingMapsService.colors[4]
                        }
                    );
                    this.map.entities.push(polygon);
                } else {
                    const pushpin = new Microsoft.Maps.Pushpin(event.location, {
                        color: BingMapsService.colors[4]
                    });
                    this.map.entities.push(pushpin);
                }

                subject.next(polygonPoints);
            }
            );
        });
    }

    endDraw(): void {
        this.load().then(() => {
            if (this.drawHandlerId) {
                Microsoft.Maps.Events.removeHandler(this.drawHandlerId);
            }
        });
    }

    addLocationPin(subject: Subject<Location>) {
        this.load().then(() => {
            this.drawHandlerId = Microsoft.Maps.Events.addHandler(
                this.map,
                'click',
                e => {
                    const loc = this.addPinInEventHandler(
                        'location-pin.png',
                        'New Location',
                        true,
                        e
                    );
                    this.searchManager.reverseGeocode({
                        location: new Microsoft.Maps.Location(loc.latitude, loc.longitude),
                        callback: (placeResult: Microsoft.Maps.Search.IPlaceResult) => {
                            loc.address = placeResult.address.formattedAddress;
                            subject.next(loc);
                        }
                    });
                }
            );
        });
    }

    addRoutePins(subject: Subject<Location[]>, initialLocations: Location[]) {
        this.load().then(() => {
            let routePoints = new Array<Location>();
            this.clear();

            if (initialLocations) {
                routePoints = initialLocations;
                this.showDispatchingPinsWithOrder(initialLocations);
            }

            this.drawHandlerId = Microsoft.Maps.Events.addHandler(
                this.map,
                'click',
                e => {
                    const loc = this.addPinInEventHandler(
                        'location-pin.png',
                        'Pin' + '(' + (routePoints.length + 1) + ')',
                        false,
                        e
                    );
                    loc.name = 'Pin';
                    routePoints.push(loc);

                    subject.next(routePoints);
                }
            );
        });
    }

    showDispatchingRoute(
        points: Point[],
        clearMap: boolean,
        colorIndex: number
    ): void {
        this.load().then(() => {
            if (clearMap) {
                this.clear();
            }

            if (points) {
                const locations = [];

                for (const p of points) {
                    locations.push(new Microsoft.Maps.Location(p.latitude, p.longitude));
                }

                const polyline = new Microsoft.Maps.Polyline(locations, {
                    strokeColor:
                        BingMapsService.tripColors[colorIndex % BingMapsService.tripColors.length],
                    strokeThickness: 2
                });

                this.map.entities.push(polyline);
                this.centerMap(points);
            }
        });
    }

    showDispatchingRoutePins(locations: Location[]) {
        this.load().then(() => {
            this.showDispatchingPinsWithOrder(locations);
        });
    }

    showPoints(points: Point[]): void {
        this.load().then(() => {
            this.clear();

            for (const p of points) {
                const location = new Microsoft.Maps.Location(p.latitude, p.longitude);
                const pushpin = new Microsoft.Maps.Pushpin(location, {
                    color: BingMapsService.colors[4]
                });

                this.map.entities.push(pushpin);
            }

            this.centerMap(points);
        });
    }

    zoomToPositionAndHighlight(position: Point, zoomLevel: number): void {
        this.load().then(() => {
            if (this.previousHighlightPoint) {
                this.map.entities.remove(this.previousHighlightPoint);
            }

            this.previousHighlightPoint = this.createPushPin(
                position,
                null,
                BingMapsService.colors[4]
            );
            this.map.entities.push(this.previousHighlightPoint);
            this.zoomToPosition(position, zoomLevel);
        });
    }

    zoomToPosition(position: Point, zoomLevel: number): void {
        this.load().then(() => {
            const points = [position];
            this.centerMap(points);
            this.map.setView({
                zoom: zoomLevel
            });
        });
    }

    showLocations(
        positions: Map<string, Location>,
        icon: string,
        centerMap: boolean
    ): void {
        this.load().then(() => {
            this.clear();

            const points = [];
            const positionsArray = Array.from(positions.entries());

            for (const key in positionsArray) {
                if (positionsArray.hasOwnProperty(key)) {
                    const loc = positionsArray[key][1];
                    points.push(this.getLocationPoint(loc));
                    const location = new Microsoft.Maps.Location(
                        loc.latitude,
                        loc.longitude
                    );
                    const pushpin = new Microsoft.Maps.Pushpin(location, {
                        title: positionsArray[key][0],
                        subTitle: loc.address,
                        icon: icon
                    });

                    this.map.entities.push(pushpin);
                }
            }

            if (centerMap) {
                this.centerMap(points);
                this.map.setView({ zoom: 10 });
            }
        });
    }

    showAssets(positions: [Asset, TrackingPoint][], centerMap: boolean): void {
        this.load().then(() => {
            this.clear();

            const points = [];

            for (const position of positions) {
                const p = position[1];
                if (p != null) {

                    points.push(p);
                    const location = new Microsoft.Maps.Location(p.latitude, p.longitude);
                    const pushpin = new Microsoft.Maps.Pushpin(location, {
                        title: position[0].id,
                        subTitle: this.timeConverter(p.time),
                        icon:
                            (position[0].assetType === AssetType.Car
                                ? '/assets/images/car-side.png'
                                : '/assets/images/truck-side.png')
                    });
                    this.map.entities.push(pushpin);
                }
            }

            if (centerMap) {
                this.centerMap(points);
            }
        });
    }

    showDevices(positions: Map<string, TrackingPoint>, centerMap: boolean): void {
        this.load().then(() => {
            this.clear();

            const points = [];

            positions.forEach((value, key) => {
                if (value != null) {
                    points.push(value);
                    const location = new Microsoft.Maps.Location(
                        value.latitude,
                        value.longitude
                    );
                    const pushpin = new Microsoft.Maps.Pushpin(location, {
                        title: key,
                        subTitle: this.timeConverter(value.time),
                        icon: '/assets/images/phone.png'
                    });
                    this.map.entities.push(pushpin);
                }

            });

            if (centerMap) {
                this.centerMap(points);
            }
        });
    }

    showTrips(trips: Trip[]): void {
        this.trip = null;
        this.load().then(() => {
            this.clear();
            let colorIndex = 0;

            const tripsLocations = [];

            for (const trip of trips) {
                this.showTrip(
                    trip,
                    false,
                    BingMapsService.tripColors[colorIndex % BingMapsService.tripColors.length]
                );

                if (
                    !tripsLocations.find(
                        location => location.id === trip.startLocation.id
                    )
                ) {
                    tripsLocations.push(trip.startLocation);
                }

                if (
                    !tripsLocations.find(location => location.id === trip.endLocation.id)
                ) {
                    tripsLocations.push(trip.endLocation);
                }
                colorIndex++;
            }

            for (const location of tripsLocations) {
                this.showLocation(
                    location,
                    'location-pin.png',
                    this.locationService.generateLocationName(location)
                );
            }
        });
    }

    showTrip(trip: Trip, singleTrip?: boolean, color?: string): void {
        this.load().then(() => {
            color = color || BingMapsService.colors[4];

            if (singleTrip) {
                this.clear();
                this.trip = trip;
            }

            // Create an infobox at the center of the map but don't show it.
            this.infobox = new Microsoft.Maps.Infobox(this.map.getCenter(), {
                visible: false
            });

            this.infobox.setMap(this.map);
            let prevEndPoint;
            let index = 1;

            const tripLegLocations = [];

            for (const leg of trip.tripLegs) {
                const array = [];

                // Put leg start stop point
                if (leg.route.length) {
                    const startPoint = this.createPushPin(
                        leg.route[0],
                        trip.id,
                        BingMapsService.colors[2],
                        index++
                    );

                    if (singleTrip) {
                        this.map.entities.push(startPoint);
                    }

                    // Draw a dotted line between the previous end point and this one.
                    if (prevEndPoint) {
                        const dottedLine = new Microsoft.Maps.Polyline(
                            [prevEndPoint.getLocation(), startPoint.getLocation()],
                            {
                                strokeThickness: 5,
                                strokeDashArray: [2, 2],
                                strokeColor: color
                            }
                        );
                        this.map.entities.add(dottedLine);
                    }
                }

                for (let i = 0; i < leg.route.length; i++) {
                    const pushpin = this.createPushPin(leg.route[i], trip.id, color);
                    array.push(pushpin.getLocation());
                }

                // Put leg end stop point
                if (leg.route.length) {
                    prevEndPoint = this.createPushPin(
                        leg.route[leg.route.length - 1],
                        trip.id,
                        BingMapsService.colors[2],
                        index++
                    );

                    if (singleTrip) {
                        this.map.entities.push(prevEndPoint);

                        if (
                            !tripLegLocations.find(
                                location => location.id === leg.startLocation.id
                            )
                        ) {
                            tripLegLocations.push(leg.startLocation);
                        }

                        if (
                            !tripLegLocations.find(
                                location => location.id === leg.endLocation.id
                            )
                        ) {
                            tripLegLocations.push(leg.endLocation);
                        }
                    }
                }

                const line = new Microsoft.Maps.Polyline(array, {
                    strokeThickness: 5,
                    strokeColor: color
                });

                this.map.entities.add(line);

                this.centerMap(array);
            }

            if (singleTrip) {
                for (const location of tripLegLocations) {
                    this.showLocation(
                        location,
                        'location-pin.png',
                        this.locationService.generateLocationName(location)
                    );
                }
            }
        });
    }

    geocodeAddress(address: string): Subject<Point> {
        const subject = new Subject<Point>();
        this.load().then(() => {
            this.searchManager.geocode({
                where: address,
                count: 1,
                callback: (geocodeResult: Microsoft.Maps.Search.IGeocodeResult) => {
                    const res = geocodeResult.results[0];
                    const p = new Point();
                    p.latitude = res.location.latitude;
                    p.longitude = res.location.longitude;
                    subject.next(p);
                }
            });
        });
        return subject;
    }
    private createPushPin(
        point: Point,
        tripId: number,
        color: string,
        title?: any,
        subTitle?: any
    ) {
        const location = new Microsoft.Maps.Location(
            point.latitude,
            point.longitude
        );
        const pushpin = new Microsoft.Maps.Pushpin(location, {
            color: color,
            title: title ? title.toString() : '',
            subTitle: subTitle ? subTitle.toString() : ''
        });

        pushpin.metadata = {
            title: 'Pin ',
            tripId: tripId,
            point: point
        };

        return pushpin;
    }

    private addPinInEventHandler(
        icon: string,
        title: string,
        clearMap: boolean,
        e: any
    ): Location {
        const event = e as Microsoft.Maps.IMouseEventArgs;
        const loc = new Location();
        loc.latitude = event.location.latitude;
        loc.longitude = event.location.longitude;
        loc.name = title;

        const pushpin = new Microsoft.Maps.Pushpin(event.location, {
            icon: icon,
            title: title
        });

        if (clearMap) {
            this.clear();
        }

        this.map.entities.push(pushpin);
        return loc;
    }

    private load(): Promise<void> {
        if (this.loadPromise) {
            return this.loadPromise;
        }

        const script = document.createElement('script');
        script.type = 'text/javascript';
        script.async = true;
        script.defer = true;

        const mapsCallback = 'bingMapsCallback';
        script.src = `https://www.bing.com/api/maps/mapcontrol?branch=release&clientApi=bingmapsfleettracker&callback=${mapsCallback}`;

        this.loadPromise = new Promise<
            void
            >((resolve: Function, reject: Function) => {
                window[mapsCallback] = () => {
                    resolve();
                };
                script.onerror = (error: Event) => {
                    console.error('maps script error' + error);
                    reject(error);
                };
            });

        document.body.appendChild(script);

        return this.loadPromise;
    }

    private centerMap(points: Point[]) {
        if (points.length) {
            points.sort(function (x, y) {
                return x.longitude - y.longitude;
            });
            const mid = Math.floor(points.length / 2);
            this.map.setView({
                center: new Microsoft.Maps.Location(
                    points[mid].latitude,
                    points[mid].longitude
                )
            });
        }
    }

    private centerMapCentroid(points: Point[]) {
        if (points.length) {
            let centerLat = 0;
            let centerLong = 0;
            points.forEach(function (p) {
                centerLat += p.latitude;
                centerLong += p.longitude;
            });

            centerLat /= points.length;
            centerLong /= points.length;

            this.map.setView({
                center: new Microsoft.Maps.Location(
                    centerLat,
                    centerLong
                )
            });
        }
    }

    private getLocationPoint(location: Location): Point {
        const p = new Point();
        p.latitude = location.latitude;
        p.longitude = location.longitude;
        return p;
    }

    private showLocation(
        endPointLocation: Location,
        icon = 'location-pin.png',
        locationName: string
    ): void {
        this.load().then(() => {
            const location = new Microsoft.Maps.Location(
                endPointLocation.latitude,
                endPointLocation.longitude
            );
            const pushpin = new Microsoft.Maps.Pushpin(location, {
                title: locationName,
                icon: icon
            });

            this.map.entities.push(pushpin);
        });
    }

    private showDispatchingPinsWithOrder(pins: Location[]) {
        let index = 1;
        pins.forEach(location =>
            this.showLocation(
                location,
                'location-pin.png',
                this.locationService.generateLocationName(location) +
                ' (' +
                index++ +
                ')'
            )
        );
    }

    private timeConverter(timestamp) {
        const a = new Date(timestamp);
        const months = [
            'Jan',
            'Feb',
            'Mar',
            'Apr',
            'May',
            'Jun',
            'Jul',
            'Aug',
            'Sep',
            'Oct',
            'Nov',
            'Dec'
        ];
        const year = a.getFullYear();
        const month = months[a.getMonth()];
        const day = a.getDate();
        const hour = a.getHours();
        const min = a.getMinutes();
        const sec = a.getSeconds();
        return `${day} ${month} ${hour}:${min}:${sec}`;
    }

    private clear() {
        this.trip = null;
        this.map.entities.clear();
    }

    private drawGeofencePolygon(geofence: Geofence, fillColor, strokeColor): Microsoft.Maps.Location[] {
        let locations = new Array<Microsoft.Maps.Location>();

        if (geofence.areaType === AreaType.Polygon) {
            for (const p of geofence.fencePolygon) {
                const location = new Microsoft.Maps.Location(p.latitude, p.longitude);
                locations.push(location);
            }
        } else if (geofence.areaType === AreaType.Circular) {
            locations = this.getCirclePolygon(geofence.fenceCenter, geofence.radiusInMeters);

            const pushpin = new Microsoft.Maps.Pushpin(
                new Microsoft.Maps.Location(geofence.fenceCenter.latitude, geofence.fenceCenter.longitude),
                {
                    color: strokeColor,
                    icon: '/assets/images/circular-geofence-center-green.svg',
                    anchor: new Microsoft.Maps.Point(12, 12)
                });
            this.map.entities.push(pushpin);
        }

        const polygon = new Microsoft.Maps.Polygon(locations,
            {
                fillColor: fillColor,
                strokeColor: strokeColor
            });

        this.map.entities.push(polygon);

        return locations;
    }

    private getCirclePolygon(center: Point, radiusInMeters): Microsoft.Maps.Location[] {
        const earthRadiusInMeters = 6371000;
        const radPerDeg = Math.PI / 180;
        const locs = new Array<Microsoft.Maps.Location>();

        if (!radiusInMeters || radiusInMeters <= 0 || radiusInMeters > earthRadiusInMeters / 2) {
            return locs;
        }

        const lat = center.latitude * radPerDeg;
        const lon = center.longitude * radPerDeg;

        const angDist = parseFloat(radiusInMeters) / earthRadiusInMeters;

        for (let i = 0; i <= 360; i++) {
            let pLatitude, pLongitude;
            const brng = i * radPerDeg;

            pLatitude = Math.asin(Math.sin(lat) * Math.cos(angDist) +
                Math.cos(lat) * Math.sin(angDist) * Math.cos(brng));

            pLongitude = lon + Math.atan2(Math.sin(brng) * Math.sin(angDist) * Math.cos(lat),
                Math.cos(angDist) - Math.sin(lat) * Math.sin(pLatitude));

            pLatitude = pLatitude / radPerDeg;
            pLongitude = pLongitude / radPerDeg;

            locs.push(new Microsoft.Maps.Location(pLatitude, pLongitude));
        }

        return locs;
    }
}
