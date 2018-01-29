import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { BingMapsService } from '../bing-maps.service';
import { environment } from '../../../environments/environment';
import { MapsService } from '../maps.service';
import { Point } from '../../shared/point';
import { SettingsService, SubscriptionKeys } from '../../core/settings.service';

@Component({
    selector: 'app-maps',
    template: '<div #map id="map"></div>',
    styleUrls: ['./maps.component.css']
})
export class MapsComponent implements OnInit {
    @ViewChild('map') mapElement: ElementRef;

    constructor(
        private bingMapsService: BingMapsService,
        private settingsService: SettingsService,
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
                    .subscribe(trip => this.bingMapsService.showTrip(trip, true));
                this.mapsService
                    .getGeofence()
                    .subscribe(geofence => this.bingMapsService.showGeofence(geofence));
                this.mapsService
                    .getDispatchingAltPoints()
                    .subscribe(points =>
                        this.bingMapsService.showDispatchingRoute(
                            points,
                            true,
                            this.mapsService.getRouteColor() + 1
                        )
                    );
                this.mapsService
                    .getDispatchingPoints()
                    .subscribe(points =>
                        this.bingMapsService.showDispatchingRoute(
                            points,
                            false,
                            this.mapsService.getRouteColor()
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
                        this.bingMapsService.zoomToPosition(position[1], 15)
                    );
                this.mapsService
                    .getDevicePosition()
                    .subscribe(position =>
                        this.bingMapsService.zoomToPosition(position[1], 15)
                    );
                this.mapsService
                    .getLocationPosition()
                    .subscribe(position =>
                        this.bingMapsService.zoomToPosition(position, 15)
                    );
                this.mapsService
                    .getItineraryPosition()
                    .subscribe(position =>
                        this.bingMapsService.zoomToPositionAndHighlight(position, 15)
                    );

                this.mapsService.getGeofenceDraw().subscribe(draw => {
                    this.bingMapsService.drawGeofence(
                        this.mapsService.getGeofenceDrawResultSubject(),
                        draw
                    );
                });
                this.mapsService
                    .getGeofenceDrawEnd()
                    .subscribe(drawEnd => this.bingMapsService.endDraw());

                this.mapsService.getLocationPinDraw().subscribe(() => {
                    this.bingMapsService.addLocationPin(
                        this.mapsService.getLocationPinResultSubject()
                    );
                });

                this.mapsService
                    .getlocationPinDrawEnd()
                    .subscribe(drawEnd => this.bingMapsService.endDraw());

                this.mapsService.getDispatchingPinsDraw().subscribe(draw => {
                    this.bingMapsService.addRoutePins(
                        this.mapsService.getDispatchingPinsResultSubject(),
                        draw
                    );
                });

                this.mapsService
                    .getDispatchingPinsDrawEnd()
                    .subscribe(drawEnd => this.bingMapsService.endDraw());

                // Use lastPositionCall to track the last position call made so that not to recenter the map
                // if this is just an update call.
                let lastPositionCall;

                this.mapsService.getAssetsPositions().subscribe(positions => {
                    this.bingMapsService.showAssets(
                        positions,
                        lastPositionCall !== 'assets'
                    );
                    lastPositionCall = 'assets';
                });

                this.mapsService.getDevicesPositions().subscribe(positions => {
                    this.bingMapsService.showDevices(
                        positions,
                        lastPositionCall !== 'devcies'
                    );
                    lastPositionCall = 'devcies';
                });

                this.mapsService.getLocationsPositions().subscribe(positions => {
                    this.bingMapsService.showLocations(
                        positions,
                        'location-pin.png',
                        true
                    );
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
