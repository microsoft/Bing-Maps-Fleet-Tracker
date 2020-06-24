// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/// <reference path='../../../node_modules/bingmaps/types/MicrosoftMaps/Microsoft.Maps.All.d.ts' />

import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

import { Asset, AssetType } from '../assets/asset';
import { Point } from '../shared/point';
import { TrackingPoint } from '../shared/tracking-point';
import { Geofence, AreaType } from '../shared/geofence';
import { Trip } from '../shared/trip';
import { Location } from '../shared/location';
import { TripService } from '../core/trip.service';
import { LocationService } from '../locations/location.service';


import { TripLeg } from '../shared/trip-leg';
import { takeWhile } from 'rxjs/operators';



@Injectable()
export class BingMapsService {
    private readonly genericColors = ['#56A6B6', '#464E50', '#56B691', '#BF8B8A', '#E95794'];
    private readonly tripColors = ['#000CCC', '#CC0200', '#73CC15', '#00EAFF'];
    private readonly yellowSpeed = 4.4;
    private readonly greenSpeed = 13.4;
    private readonly geofenceGreenBlue = 'rgba(86,182,145,0.4)';
    private readonly geofencePink = 'rgba(233,87,148,0.4)';
    pinCount = 1;

    private map: Microsoft.Maps.Map;
    private loadPromise: Promise<void>;
    private searchManager: Microsoft.Maps.Search.SearchManager;
    private spatialMath;
    private drawHandlerId;

    private geofencesLayer: Microsoft.Maps.Layer;
    private assetsLayer: Microsoft.Maps.Layer;
    private devicesLayer: Microsoft.Maps.Layer;
    private pointsLayer: Microsoft.Maps.Layer;
    private tripsLayer: Microsoft.Maps.Layer;
    private locationsLayer: Microsoft.Maps.Layer;

    constructor(private tripService: TripService) { }

    /* GENERIC FUNCTIONS */
    init(element: HTMLElement, options: Microsoft.Maps.IMapLoadOptions): void {
        this.load().then(() => {
            this.map = new Microsoft.Maps.Map(element, options);

            if (!this.searchManager) {
                Microsoft.Maps.loadModule('Microsoft.Maps.Search', () => {
                    this.searchManager = new Microsoft.Maps.Search.SearchManager(this.map);
                });
            }

            if (!this.spatialMath) {
                Microsoft.Maps.loadModule('Microsoft.Maps.SpatialMath', () => {
                    this.spatialMath = Microsoft.Maps.SpatialMath;
                });
            }

            this.geofencesLayer = new Microsoft.Maps.Layer('geofencesLayer');
            this.assetsLayer = new Microsoft.Maps.Layer('assetsLayer');
            this.devicesLayer = new Microsoft.Maps.Layer('devicesLayer');
            this.tripsLayer = new Microsoft.Maps.Layer('tripsLayer');
            this.locationsLayer = new Microsoft.Maps.Layer('locationsLayer');
            this.pointsLayer = new Microsoft.Maps.Layer('pointsLayer');

            this.map.layers.insert(this.geofencesLayer);
            this.map.layers.insert(this.assetsLayer);
            this.map.layers.insert(this.devicesLayer);
            this.map.layers.insert(this.tripsLayer);
            this.map.layers.insert(this.locationsLayer);
            this.map.layers.insert(this.pointsLayer);
        });
    }

    endCurrentDraw(): void {
        if (this.drawHandlerId) {
            Microsoft.Maps.Events.removeHandler(this.drawHandlerId);
            this.drawHandlerId = null;
        }
    }

    centerMap(point: Point, zoom = null) {
        if (point) {
            this.map.setView({
                center: new Microsoft.Maps.Location(
                    point.latitude,
                    point.longitude
                ),
                zoom: zoom
            });
        }
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

    private centerMapOnMedian(points: Point[]) {
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

    private centerMapOnMean(points: Point[]) {
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

    private timeConverter(timestamp) {
        const a = new Date(timestamp);
        return a.toLocaleString('en-US',
            { month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric' });
    }

    private resetMap() {
        this.geofencesLayer.setVisible(false);
        this.assetsLayer.setVisible(false);
        this.devicesLayer.setVisible(false);
        this.pointsLayer.setVisible(false);
        this.tripsLayer.setVisible(false);
        this.locationsLayer.setVisible(false);
    }
    /* END GENERIC FUNCTIONS */

    /* GEOFENCES FUNCTIONS */
    showGeofences(geofences: Geofence[]): void {
        this.load().then(() => {
            this.resetMap();
            this.assetsLayer.setVisible(true);
            this.geofencesLayer.setVisible(true);
            this.geofencesLayer.clear();

            let lastGeofencePolygon = new Array<Microsoft.Maps.Location>();
            for (const geofence of geofences) {
                lastGeofencePolygon = this.showGeofencePolygon(geofence, this.geofenceGreenBlue, this.genericColors[0]);
            }

            this.centerMapOnMean(lastGeofencePolygon);
        });
    }

    showGeofence(geofence: Geofence): void {
        this.load().then(() => {
            this.resetMap();
            this.assetsLayer.setVisible(true);
            this.geofencesLayer.setVisible(true);
            this.geofencesLayer.clear();

            const polygonLocations = this.showGeofencePolygon(geofence, this.geofenceGreenBlue, this.genericColors[0]);

            this.centerMapOnMean(polygonLocations);
        });
    }

    drawCircularGeofence(subject: Subject<Point>, initialCenter: Point, initialRadius: number, radius: Observable<number>) {
        this.load().then(() => {
            const tempGeofence = new Geofence();
            tempGeofence.fenceCenter = initialCenter || new Point();
            tempGeofence.radiusInMeters = initialRadius || 1;
            tempGeofence.areaType = AreaType.Circular;

            this.resetMap();
            this.geofencesLayer.setVisible(true);
            this.geofencesLayer.clear();

            // Draw initial geofence area value if it exists
            if (initialCenter && initialCenter.latitude && initialCenter.longitude && initialRadius) {
                this.showGeofencePolygon(tempGeofence, this.geofenceGreenBlue, this.genericColors[0]);
            }

            // If new center set, update temp geofence and redraw
            this.drawHandlerId = Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                const event = e as Microsoft.Maps.IMouseEventArgs;

                tempGeofence.fenceCenter.latitude = event.location.latitude;
                tempGeofence.fenceCenter.longitude = event.location.longitude;

                this.geofencesLayer.clear();
                this.showGeofencePolygon(
                    tempGeofence, this.geofencePink, this.genericColors[4], '/assets/images/circular-geofence-center-pink.svg');

                subject.next(tempGeofence.fenceCenter);
            });

            // If radius changed, update temp geofence and redraw if center is specified
            // Stop subscribing to updates if the drawHandler is changed
            const currentDrawHandlerId = this.drawHandlerId;
            radius
                .pipe(takeWhile(() => currentDrawHandlerId === this.drawHandlerId))
                .subscribe(r => {
                    tempGeofence.radiusInMeters = r;
                    if (tempGeofence.fenceCenter && tempGeofence.fenceCenter.latitude && tempGeofence.fenceCenter.longitude) {
                        this.geofencesLayer.clear();
                        this.showGeofencePolygon(
                            tempGeofence, this.geofencePink, this.genericColors[4], '/assets/images/circular-geofence-center-pink.svg');
                    }
                });
        });
    }

    drawPolygonGeofence(subject: Subject<Point[]>, initialPoints: Point[]) {
        this.load().then(() => {
            const tempGeofence = new Geofence();
            tempGeofence.fencePolygon = initialPoints || new Array<Point>();
            tempGeofence.areaType = AreaType.Polygon;

            this.resetMap();
            this.geofencesLayer.setVisible(true);
            this.geofencesLayer.clear();

            // Draw initial geofence area value if it exists
            if (initialPoints && initialPoints.length > 1) {
                this.showGeofencePolygon(tempGeofence, this.geofenceGreenBlue, this.genericColors[0]);
            }

            // If new point added, update temp geofence and redraw
            this.drawHandlerId = Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                const event = e as Microsoft.Maps.IMouseEventArgs;
                tempGeofence.fencePolygon.push(event.location);

                this.geofencesLayer.clear();
                if (tempGeofence.fencePolygon.length > 1) {
                    this.showGeofencePolygon(tempGeofence, this.geofencePink, this.genericColors[4]);
                } else {
                    const pushpin = new Microsoft.Maps.Pushpin(event.location, {
                        color: this.genericColors[4]
                    });
                    this.geofencesLayer.add(pushpin);
                }

                subject.next(tempGeofence.fencePolygon);
            }
            );
        });
    }

    private showGeofencePolygon(
        geofence: Geofence,
        fillColor,
        strokeColor,
        circularIcon = '/assets/images/circular-geofence-center-green.svg')
        : Microsoft.Maps.Location[] {

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
                    icon: circularIcon,
                    anchor: new Microsoft.Maps.Point(12, 12)
                });
            this.geofencesLayer.add(pushpin);
        }

        const polygon = new Microsoft.Maps.Polygon(locations,
            {
                fillColor: fillColor,
                strokeColor: strokeColor
            });

        this.geofencesLayer.add(polygon);

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
    /* END GEOFENCES FUNCTIONS */

    /* LOCATION FUNCTIONS */
    drawLocationPin(subject: Subject<Location>, location: Location) {
        this.load().then(() => {
            const tempLocation = new Location();
            tempLocation.name = 'New Location';

            if (location && location.name) {
                tempLocation.name = location.name;
            }

            this.resetMap();
            this.locationsLayer.setVisible(true);
            this.locationsLayer.clear();

            this.drawHandlerId = Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                const event = e as Microsoft.Maps.IMouseEventArgs;
                tempLocation.latitude = event.location.latitude;
                tempLocation.longitude = event.location.longitude;
                this.locationsLayer.clear();
                this.showLocation(tempLocation);

                this.searchManager.reverseGeocode({
                    location: event.location,
                    callback: (placeResult: Microsoft.Maps.Search.IPlaceResult) => {
                        tempLocation.address = placeResult.address.formattedAddress;
                        this.locationsLayer.clear();
                        this.showLocation(tempLocation);
                        subject.next(tempLocation);
                    }
                });
            }
            );
        });
    }



    drawDispatchingRoute(subject: Subject<Location[]>, initialLocations: Location[]) {
        this.load().then(() => {
            const tempRoutePoints = initialLocations || new Array<Location>();

            this.resetMap();
            this.assetsLayer.setVisible(true);
            this.locationsLayer.setVisible(true);
            this.locationsLayer.clear();

            if (initialLocations) {
                this.showDispatchingRoutePins(tempRoutePoints);
            } else {
                this.pinCount = 1;
            }

            this.drawHandlerId = Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                const event = e as Microsoft.Maps.IMouseEventArgs;

                const newLocation = new Location();
                newLocation.name = 'Pin' + '(' + (this.pinCount) + ')';
                this.pinCount += 1;
                newLocation.latitude = event.location.latitude;
                newLocation.longitude = event.location.longitude;
                this.searchManager.reverseGeocode({
                    location: event.location,
                    callback: (placeResult: Microsoft.Maps.Search.IPlaceResult) => {
                        newLocation.address = placeResult.address.formattedAddress;
                        this.showLocation(newLocation);
                        tempRoutePoints.push(newLocation);
                        subject.next(tempRoutePoints);
                    }
                });
            }
            );
        });
    }

    showDispatchingRoute(points: Point[], clearMap: boolean, colorIndex: number, Thickness: number): void {
        this.load().then(() => {
            if (clearMap) {
                this.locationsLayer.clear();
            }

            if (points && points.length >= 1) {
                const locations = [];

                for (const p of points) {
                    locations.push(new Microsoft.Maps.Location(p.latitude, p.longitude));
                }

                const polyline = new Microsoft.Maps.Polyline(locations, {
                    strokeColor:
                        this.tripColors[colorIndex % this.tripColors.length],
                    strokeThickness: Thickness
                });

                this.locationsLayer.add(polyline);
                this.centerMap(points[0]);
            }
        });
    }

    showDispatchingRoutePins(locations: Location[]) {
        this.load().then(() => {
            locations.forEach(location => {
                this.showLocation(location);
            });
        });
    }

    showLocations(locations: Location[]) {
        this.load().then(() => {
            this.resetMap();
            this.assetsLayer.setVisible(true);
            this.locationsLayer.setVisible(true);
            this.locationsLayer.clear();

            locations.forEach(location => {
                this.showLocation(location);
            });

            this.centerMapOnMedian(locations);
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

    private showLocation(location: Location, icon = '/assets/images/location-pin.png') {
        const mapLocation = new Microsoft.Maps.Location(
            location.latitude,
            location.longitude
        );
        const pushpin = new Microsoft.Maps.Pushpin(mapLocation, {
            title: location.name,
            subTitle: location.address,
            icon: icon
        });

        this.locationsLayer.add(pushpin);
    }

    private getNormalizedLocationName(location: Location) {
        if (location.name === 'Auto-Generated Location') {
            return 'Location ' + location.id;
        } else {
            return location.name;
        }
    }
    /* END LOCATION FUNCTIONS */

    /* POINT FUNCTIONS */
    showPoints(data: { points: Point[]; snappedPoints: boolean; }): void {
        let points = data["points"];
        this.load().then(() => {
            this.resetMap();
            this.pointsLayer.setVisible(true);
            this.pointsLayer.clear();
            if (data["snappedPoints"]) {
                points.forEach(p => this.showPoint(p, this.genericColors[2]));
            } else {
                points.forEach(p => this.showPoint(p, this.genericColors[4]));
            }

            this.centerMapOnMedian(points);
        });
    }

    private showPoint(point: Point, color) {
        const location = new Microsoft.Maps.Location(point.latitude, point.longitude);
        const pushpin = new Microsoft.Maps.Pushpin(location, {
            color: color
            //  BingMapsService.colors[4]
        });

        this.pointsLayer.add(pushpin);
    }

    computeDistanceBetween(point1: Point, point2: Point) {
        let p1 = new Microsoft.Maps.Location(point1.latitude, point1.longitude);
        let p2 = new Microsoft.Maps.Location(point2.latitude, point2.longitude);

        return this.spatialMath.getDistanceTo(p1, p2, Microsoft.Maps.SpatialMath.DistanceUnits.Kilometers)
    }
    /* END POINT FUNCTIONS */

    /* ASSET FUNCTIONS */
    showAssets(positions: [Asset, TrackingPoint][], centerMap: boolean, clearMap: boolean): void {
        this.load().then(() => {
            if (clearMap) {
                this.resetMap();
                this.assetsLayer.setVisible(true);
            }
            this.assetsLayer.clear();

            const points = [];

            for (const position of positions) {
                const p = position[1];
                if (p != null) {

                    points.push(p);
                    const location = new Microsoft.Maps.Location(p.latitude, p.longitude);
                    const pushpin = new Microsoft.Maps.Pushpin(location, {
                        title: position[0].name,
                        subTitle: this.timeConverter(p.time),
                        icon:
                            (position[0].assetType === AssetType.Car
                                ? '/assets/images/car-side.png'
                                : '/assets/images/truck-side.png')
                    });
                    this.assetsLayer.add(pushpin);
                }
            }

            if (centerMap) {
                this.centerMapOnMedian(points);
            }
        });
    }
    /* END ASSET FUNCTIONS */

    /* DEVICE FUNCTIONS */
    showDevices(positions: Map<string, TrackingPoint>, centerMap: boolean, clearMap: boolean): void {
        this.load().then(() => {
            if (clearMap) {
                this.resetMap();
                this.devicesLayer.setVisible(true);
            }
            this.devicesLayer.clear();

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
                    this.devicesLayer.add(pushpin);
                }

            });

            if (centerMap) {
                this.centerMapOnMedian(points);
            }
        });
    }
    /* END DEVICE FUNCTIONS */

    /* TRIP FUNCTIONS*/
    showTrips(trips: Trip[]): void {
        this.load().then(() => {
            this.resetMap();
            this.tripsLayer.setVisible(true);
            this.tripsLayer.clear();

            let colorIndex = 0;
            for (const trip of trips) {
                this.showSimplifiedTrip(trip, this.tripColors[colorIndex++ % this.tripColors.length]);
            }
        });
    }

    showTrip(trip: Trip, color?: string): void {
        this.load().then(() => {
            this.resetMap();
            this.tripsLayer.setVisible(true);
            this.tripsLayer.clear();

            this.showDetailedTrip(trip);

            this.drawHandlerId = Microsoft.Maps.Events.addHandler(this.map, 'click', e => {
                const event = e as Microsoft.Maps.IMouseEventArgs;
                this.tripService.getPoints(
                    trip.id,
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

                        const location = new Microsoft.Maps.Location(point.latitude, point.longitude);
                        const pushPin = new Microsoft.Maps.Pushpin(location, {
                            color: pinColor,
                            title: this.timeConverter(point.time),
                            subTitle: `speed: ${Math.round(point.speed * 3.6)} km/hr, accuracy: ${point.accuracy}`
                        });
                        this.tripsLayer.add(pushPin);
                    }
                });
            });
        });
    }

    private showDetailedTrip(trip: Trip) {
        let colorIndex = 0;
        let previousTripLeg: TripLeg;
        for (const leg of trip.tripLegs) {
            const tripLegLocations = [];

            // Show the starting stop in a leg if its different from the ending stop of the last
            if (!previousTripLeg || previousTripLeg.endLocation.id !== leg.startLocation.id) {
                this.showTripLocation(leg.startLocation);
                tripLegLocations.push(new Microsoft.Maps.Location(
                    leg.startLocation.latitude,
                    leg.startLocation.longitude
                ));
            }

            // Add all route points
            for (let i = 0; i < leg.route.length; i++) {
                tripLegLocations.push(new Microsoft.Maps.Location(
                    leg.route[i].latitude,
                    leg.route[i].longitude
                ));
            }

            // Show the ending stop in a leg
            this.showTripLocation(leg.endLocation);
            tripLegLocations.push(new Microsoft.Maps.Location(
                leg.endLocation.latitude,
                leg.endLocation.longitude
            ));

            // Show every leg in a different color
            const line = new Microsoft.Maps.Polyline(tripLegLocations, {
                strokeThickness: 5,
                strokeColor: this.tripColors[colorIndex++ % this.tripColors.length]
            });
            this.tripsLayer.add(line);

            // Draw a dotted line between trip leg stop locations, if they are not the same.
            if (previousTripLeg && previousTripLeg.endLocation.id !== leg.startLocation.id) {
                const startPoint = new Microsoft.Maps.Location(
                    leg.startLocation.latitude,
                    leg.startLocation.longitude
                );

                const previousLocation = new Microsoft.Maps.Location(
                    previousTripLeg.endLocation.latitude,
                    previousTripLeg.endLocation.longitude);

                const dottedLine = new Microsoft.Maps.Polyline(
                    [previousLocation, startPoint],
                    {
                        strokeThickness: 5,
                        strokeDashArray: [2, 2],
                        strokeColor: this.genericColors[4]
                    }
                );
                this.tripsLayer.add(dottedLine);
            }

            previousTripLeg = leg;
        }

        this.centerMap(trip.startLocation);
    }

    private showSimplifiedTrip(trip: Trip, tripColor) {
        tripColor = tripColor || this.genericColors[4];

        const tripLegLocations = [];

        // Show the first stop in a trip
        this.showTripLocation(trip.startLocation);
        tripLegLocations.push(new Microsoft.Maps.Location(
            trip.startLocation.latitude,
            trip.startLocation.longitude
        ));

        let previousTripLeg: TripLeg;
        for (const leg of trip.tripLegs) {
            // Add all route points
            for (let i = 0; i < leg.route.length; i++) {
                tripLegLocations.push(new Microsoft.Maps.Location(
                    leg.route[i].latitude,
                    leg.route[i].longitude
                ));
            }

            // Draw a dotted line between trip leg stop locations, if they are not the same.
            if (previousTripLeg && previousTripLeg.endLocation.id !== leg.startLocation.id) {
                const startPoint = new Microsoft.Maps.Location(
                    leg.startLocation.latitude,
                    leg.startLocation.longitude
                );

                const previousLocation = new Microsoft.Maps.Location(
                    previousTripLeg.endLocation.latitude,
                    previousTripLeg.endLocation.longitude);

                const dottedLine = new Microsoft.Maps.Polyline(
                    [previousLocation, startPoint],
                    {
                        strokeThickness: 5,
                        strokeDashArray: [2, 2],
                        strokeColor: tripColor
                    }
                );
                this.tripsLayer.add(dottedLine);
            }

            previousTripLeg = leg;
        }

        // Show the last stop in a trip
        this.showTripLocation(trip.endLocation);
        tripLegLocations.push(new Microsoft.Maps.Location(
            trip.endLocation.latitude,
            trip.endLocation.longitude
        ));

        const line = new Microsoft.Maps.Polyline(tripLegLocations, {
            strokeThickness: 5,
            strokeColor: tripColor
        });

        this.tripsLayer.add(line);
        this.centerMap(tripLegLocations[0]);
    }

    private showTripLocation(location: Location, icon = '/assets/images/location-pin.png') {
        const mapLocation = new Microsoft.Maps.Location(
            location.latitude,
            location.longitude
        );
        const pushpin = new Microsoft.Maps.Pushpin(mapLocation, {
            title: location.name,
            subTitle: location.address,
            icon: icon
        });

        this.tripsLayer.add(pushpin);
    }
    /* END TRIP FUNCTIONS*/
}
