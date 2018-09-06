// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/// <reference path="../../scripts/MicrosoftMaps/Modules/Directions.d.ts"/>
/// <reference path="../../scripts/MicrosoftMaps/Modules/Search.d.ts"/>

import { Injectable } from '@angular/core';
import { Platform } from 'ionic-angular';
import { Logger } from 'angular2-logger/core';
import { Network } from 'ionic-native';
import { Http, Headers } from '@angular/http';
import { SettingsService, Settings } from './settings-service';
import { BingMapsService } from './bing-maps-service';
import { BackgroundTrackerService } from './background-tracker-service';
import 'rxjs/add/operator/toPromise';
import { SignalrClientServiceProvider } from './signalr-client-service';
import { Point } from '../shared/point';
import { DispatchingParameters } from '../shared/dispatching-parameters';

declare var navigator;

@Injectable()
export class MapHostService {

  private readonly yellowSpeed = 4.4;
  private readonly greenSpeed = 13.4;
  private mapElement: HTMLElement;
  private map: Microsoft.Maps.Map;
  private tracksLayer: Microsoft.Maps.Layer;
  private dispatchLayer: Microsoft.Maps.Layer;
  private tracksLayerActive: boolean = true;
  private dispatchLayerActive: boolean = true;
  private lastLocation;
  private directionsManager: Microsoft.Maps.Directions.DirectionsManager;
  searchManager: Microsoft.Maps.Search.SearchManager;
  private lastDispatch: DispatchingParameters;
  private onDevice;
  private isOnline;
  private deferMapsCreation = false;
  private connectSub;
  private disconnectSub;

  constructor(
    private platform: Platform,
    private logger: Logger,
    private http: Http,
    private bingMapsService: BingMapsService,
    private settingsService: SettingsService,
    private backgroundTrackerService: BackgroundTrackerService,
    private signalrservice: SignalrClientServiceProvider,
  ) {
    this.onDevice = this.platform.is('cordova');
  }

  initialize(mapElement: HTMLElement) {
    this.isOnline = !this.onDevice || navigator.connection.type !== 'none';
    this.mapElement = mapElement;
    this.connectSub = Network.onConnect().subscribe(() => this.connected());
    this.disconnectSub = Network.onConnect().subscribe(() => this.disconnected());

    if (this.isOnline) {
      this.getMapsKey().then(key => {
        this.createMap(key);
      });
    }
    else {
      this.deferMapsCreation = true;
      this.logger.info('deferring map creation');
    }
  }

  uninitialize() {
    this.connectSub.unsubscribe();
    this.disconnectSub.unsubscribe();
  }

  showLocation() {
    if (this.lastLocation) {
      this.map.setView({
        center: this.lastLocation
      });
    } else {
      alert('No location accuired yet!');
    }
  }

  dispatchService() {
    if (this.dispatchLayerActive) {
      this.directionsManager.clearAll();
      this.centerMap(this.lastLocation);
      this.dispatchLayerActive = !this.dispatchLayerActive;
    } else {
      this.getDirections(this.lastDispatch);
    }
  }

  togglePins() {
    this.logger.info('toggle pins');

    if (this.tracksLayerActive) {
      this.map.layers.remove(this.tracksLayer);
    } else {
      this.map.layers.insert(this.tracksLayer);
    }

    this.tracksLayerActive = !this.tracksLayerActive;
  }

  private connected() {
    this.isOnline = true;
    if (this.deferMapsCreation) {
      this.getMapsKey().then(key => {
        this.createMap(key);
      });

      this.deferMapsCreation = false;
    }
  }

  private disconnected() {
    this.isOnline = false;
  }

  private createMap(credentials: string) {
    this.bingMapsService.createMap(this.mapElement, {
      credentials: credentials,
      showDashboard: false,
      showZoomButtons: false,
      enableClickableLogo: false
    }).then((map) => {
      this.logger.info('map created');
      this.map = map;

      this.tracksLayer = new Microsoft.Maps.Layer();
      this.map.layers.insert(this.tracksLayer);

      this.dispatchLayer = new Microsoft.Maps.Layer();
      this.map.layers.insert(this.dispatchLayer);

      Microsoft.Maps.loadModule('Microsoft.Maps.Directions', () => {
        this.directionsManager = new Microsoft.Maps.Directions.DirectionsManager(map);
        this.startListening();
      });

      Microsoft.Maps.loadModule('Microsoft.Maps.Search', () => {
        this.searchManager = new Microsoft.Maps.Search.SearchManager(map);
      });

    }).catch((er) => {
      this.logger.error('map creation request failed ' + er);;
    });
  }

  private startListening() {
    this.backgroundTrackerService.locations.subscribe((location) => {

      // ok to execute location updates even if offline since the maps control seems to 
      // buffer it's operations for when it's back online

      if (!this.lastLocation) {
        this.map.setView({
          center: new Microsoft.Maps.Location(location.latitude, location.longitude)
        });
      }

      this.markTrack(location);
      this.lastLocation = location;
    });
  }

  getDirections(params: DispatchingParameters) {
    this.dispatchLayerActive = true;
    if (params == null)
      return;

    this.directionsManager.clearAll();
    this.lastDispatch = params;

    this.directionsManager.addWaypoint(new Microsoft.Maps.Directions.Waypoint({ location: new Microsoft.Maps.Location(this.lastLocation.latitude, this.lastLocation.longitude) }), 0);
    this.centerMap(this.lastLocation);

    params.wayPoints.forEach(pin => {
      this.directionsManager.addWaypoint(new Microsoft.Maps.Directions.Waypoint({ location: new Microsoft.Maps.Location(pin.latitude, pin.longitude) }));
    });

    this.directionsManager.setRequestOptions({
      routeDraggable: false,
      vehicleSpec: {
        vehicleHeight: params.loadedHeight,
        vehicleWidth: params.loadedWidth,
        vehicleLength: params.loadedLength,
        vehicleWeight: params.loadedWeight,
        vehicleAvoidCrossWind: params.avoidCrossWind,
        vehicleAvoidGroundingRisk: params.avoidGroundingRisk
      }
    });

    this.directionsManager.calculateDirections();
  }

  clearDirections() {
    this.lastDispatch = null;
    this.directionsManager.clearAll();
    this.centerMap(this.lastLocation);
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

  private markTrack(location) {
    let pinColor;

    if (location.speed >= this.greenSpeed) {
      pinColor = 'green';
    } else if (location.speed < this.greenSpeed && location.speed >= this.yellowSpeed) {
      pinColor = 'yellow';
    } else {
      pinColor = 'red';
    }

    var pushpin = new Microsoft.Maps.Pushpin(
      new Microsoft.Maps.Location(location.latitude, location.longitude), { color: pinColor });

    this.tracksLayer.add(pushpin);
  }

  private getMapsKey() {
    var backendUrlPromise = this.settingsService.get(Settings.BackendUrl);
    var tokenPromise = this.settingsService.get(Settings.SecurityToken);

    return Promise.all([backendUrlPromise, tokenPromise]).then(values => {
      var headers = new Headers();
      headers.append('Authorization', 'Bearer ' + values[1]);
      return this.http.get(`${values[0]}/api/settings/subscriptionkeys`, { headers: headers }).toPromise()
        .then(response => {
          let keys = response.json();

          let bingMapsKey = '';
          for (var key of keys) {
            if (key['id'] === 'BingMaps') {
              bingMapsKey = key['keyValue'];
              break;
            }
          }

          this.signalrservice.connect();
          return bingMapsKey;
        });
    })
  }
}
