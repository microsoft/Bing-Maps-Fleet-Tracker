import { Injectable } from '@angular/core';
import { Platform } from 'ionic-angular';
import { Logger } from 'angular2-logger/core';
import { Network } from 'ionic-native';
import { Http, Headers } from '@angular/http';

import { SettingsService, Settings } from './settings-service';
import { BingMapsService } from './bing-maps-service';
import { BackgroundTrackerService } from './background-tracker-service';

import 'rxjs/add/operator/toPromise';

declare var navigator;

@Injectable()
export class MapHostService {

  private readonly yellowSpeed = 4.4;
  private readonly greenSpeed = 13.4;

  private mapElement: HTMLElement;
  private map: Microsoft.Maps.Map;
  private tracksLayer: Microsoft.Maps.Layer;

  private tracksLayerActive: boolean = true;
  private lastLocation;

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
    private backgroundTrackerService: BackgroundTrackerService) {

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
      this.startListening();
    }).catch(() => {
      this.logger.error('map creation request failed');;
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

          return bingMapsKey;
        });
    })
  }
}
