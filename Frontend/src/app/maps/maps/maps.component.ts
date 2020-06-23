// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { BingMapsService } from '../bing-maps.service';
import { environment } from '../../../environments/environment';
import { MapsService } from '../maps.service';
import { Point } from '../../shared/point';
import { SettingsService, SubscriptionKeys } from '../../core/settings.service';
import { AssetService } from '../../assets/asset.service';
import { Asset } from '../../assets/asset';
import { TrackingPoint } from '../../shared/tracking-point';

@Component({
    selector: 'app-maps',
    template: '<div #map id="map"></div>',
    styleUrls: ['./maps.component.css']
})
export class MapsComponent implements OnInit {
    @ViewChild('map', { static: false }) mapElement: ElementRef;

    constructor(
        private bingMapsService: BingMapsService,
        private settingsService: SettingsService,
        private assetsService: AssetService,
        private mapsService: MapsService, ) { }

    ngOnInit() {
        this.settingsService
            .getSubscriptionKey(SubscriptionKeys.BingMaps)
            .subscribe(key => {
                this.bingMapsService.init(this.mapElement.nativeElement, {
                    credentials: key.keyValue,
                    showDashboard: true,
                    showZoomButtons: true,
                    disableBirdseye: true,
                    disableStreetside: true,
                    showLocateMeButton: false,
                    enableClickableLogo: false
                });

                this.mapsService.getGeocodeQuery()
                    .subscribe(address => this.bingMapsService.geocodeAddress(address)
                        .subscribe(point => this.mapsService.geocodeResult(point))
                    );

                this.mapsService
                    .getPoints()
                    .subscribe(points => this.bingMapsService.showPoints(points));
                this.mapsService
                    .getTrips()
                    .subscribe(trips => this.bingMapsService.showTrips(trips));
                this.mapsService
                    .getTrip()
                    .subscribe(trip => this.bingMapsService.showTrip(trip));
                this.mapsService
                    .getGeofence()
                    .subscribe(geofence => this.bingMapsService.showGeofence(geofence));
                this.mapsService
                    .getDispatchingAltPoints()
                    .subscribe(points =>
                        this.bingMapsService.showDispatchingRoute(
                            points,
                            true,
                            this.mapsService.getRouteColor() + 1,
                            2
                        )
                    );
                this.mapsService
                    .getDispatchingPoints()
                    .subscribe(points =>
                        this.bingMapsService.showDispatchingRoute(
                            points,
                            false,
                            this.mapsService.getRouteColor(),
                            4
                        )
                    );
                this.mapsService
                    .getDispatchingPins()
                    .subscribe(pins =>
                        this.bingMapsService.showDispatchingRoutePins(pins)
                    );
                this.mapsService
                    .getAssetPosition()
                    .subscribe(position =>
                        this.bingMapsService.centerMap(position[1], 15)
                    );
                this.mapsService
                    .getDevicePosition()
                    .subscribe(position =>
                        this.bingMapsService.centerMap(position[1], 15)
                    );
                this.mapsService
                    .getLocationPosition()
                    .subscribe(position =>
                        this.bingMapsService.centerMap(position, 15)
                    );
                this.mapsService
                    .getItineraryPosition()
                    .subscribe(position =>
                        this.bingMapsService.centerMap(position, 15)
                    );

                this.mapsService.getGeofenceCircularDraw().subscribe(draw => {
                    this.bingMapsService.drawCircularGeofence(
                        this.mapsService.getGeofenceCircularDrawResultSubject(),
                        draw[0],
                        draw[1],
                        this.mapsService.getCircularGeofenceRadiusChange()
                    );
                });

                this.mapsService.getGeofencePolygonDraw().subscribe(draw => {
                    this.bingMapsService.drawPolygonGeofence(
                        this.mapsService.getGeofencePolygonDrawResultSubject(),
                        draw
                    );
                });
                this.mapsService
                    .getCurrentDrawEnd()
                    .subscribe(drawEnd => this.bingMapsService.endCurrentDraw());

                this.mapsService.getLocationPinDraw().subscribe((location) => {
                    this.bingMapsService.drawLocationPin(
                        this.mapsService.getLocationPinResultSubject(),
                        location
                    );
                });

                this.mapsService.getDispatchingPinsDraw().subscribe(draw => {
                    this.bingMapsService.drawDispatchingRoute(
                        this.mapsService.getDispatchingPinsResultSubject(),
                        draw
                    );
                });

                let assets = new Array<Asset>();
                this.assetsService.getAssets().subscribe(a => assets = a);

                // Use lastPositionCall to track the last position call made so that not to recenter the map
                // if this is just an update call.
                let lastPositionCall;

                // Do the mapping in this layer to avoid having to redo in multiple components
                this.mapsService.getAssetsPositions().subscribe(data => {
                    const positions = data[0];
                    const mappedAssets = new Array<[Asset, TrackingPoint]>();
                    for (const k of Object.keys(positions)) {
                        const value = positions[k];
                        const asset = assets.find(val => val.name === k);
                        if (asset) {
                            mappedAssets.push([asset, value]);
                        }
                    }

                    this.bingMapsService.showAssets(
                        mappedAssets,
                        !data[1] && lastPositionCall !== 'assets',
                        !data[1]
                    );

                    lastPositionCall = 'assets';
                });

                this.mapsService.getDevicesPositions().subscribe(data => {
                    this.bingMapsService.showDevices(
                        data[0],
                        !data[1] && lastPositionCall !== 'devices',
                        !data[1]
                    );

                    lastPositionCall = 'devices';
                });

                this.mapsService.getLocationsPositions().subscribe(positions => {
                    this.bingMapsService.showLocations(positions);
                });

                this.mapsService.getGeofences().subscribe(geofences => {
                    this.bingMapsService.showGeofences(geofences);
                    lastPositionCall = 'geofences';
                });
            }, error => {
                console.log(error);
                this.bingMapsService.init(this.mapElement.nativeElement, {
                    credentials: ''
                });
            });
    }
}
