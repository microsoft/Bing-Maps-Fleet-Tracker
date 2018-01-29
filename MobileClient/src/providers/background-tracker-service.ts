import { Injectable } from '@angular/core';
import { Platform } from 'ionic-angular';
import { Observable } from 'rxjs/Observable';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Logger } from 'angular2-logger/core';

import { SettingsService, Settings } from '../providers/settings-service';

declare var device;
declare var backgroundGeolocation;

@Injectable()
export class BackgroundTrackerService {

  private locationsSubject: ReplaySubject<any> = new ReplaySubject<any>();
  locations: Observable<any> = this.locationsSubject.asObservable();

  isTracking: boolean = false;

  private startPromise: Promise<any>;

  private defaultBgOptions: any = {
    stationaryRadius: 50, //Stationary radius in meters. When stopped, the minimum distance the device must move beyond the stationary location for aggressive background-tracking to engage.
    distanceFilter: 50, //The minimum distance (measured in meters) a device must move horizontally before an update event is generated. @see Apple docs.
    desiredAccuracy: 100, //Desired accuracy in meters. Possible values [0, 10, 100, 1000]. The lower the number, the more power devoted to GeoLocation resulting in higher accuracy readings. 1000 results in lowest power drain and least accurate readings. @see Apple docs
    stopOnTerminate: false, //Enable this in order to force a stop() when the application terminated (e.g. on iOS, double-tap home button, swipe away the app). (default true)

    //
    // iOS only section
    activityType: 'AutomotiveNavigation', //iOS [AutomotiveNavigation, OtherNavigation, Fitness, Other] Presumably, this affects iOS GPS algorithm. @see Apple docs for more information
    pauseLocationUpdates: false, //iOS Pauses location updates when app is paused (default: true)
    saveBatteryOnBackground: true, //iOS Switch to less accurate significant changes and region monitory when in background (default)
    //
    // Android only section
    locationProvider: 0, //Set location provider @see PROVIDERS.md
    interval: 10, //Android
    fastestInterval: 5, //Android
    activitiesInterval: 10, //Android
    startForeground: true, //Androind If false location service will not be started in foreground and no notification will be shown. (default true)
    startOnBoot: true, //Android Start background service on device boot. (default false)
    stopOnStillActivity: true //Android stop() is forced, when the STILL activity is detected (default is true)
  };

  constructor(
    private platform: Platform,
    private logger: Logger,
    private settingsService: SettingsService) {
  }

  startTracking(): Promise<void> {
    this.logger.info('tracking started');

    if (this.isTracking) {
      return this.startPromise;
    }

    this.isTracking = true;

    let startFinished;
    this.startPromise = new Promise((resolve) => { startFinished = resolve; });

    this.logger.info('checking bgOptions settings');

    let urlPromise = this.settingsService.get(Settings.BackendUrl);
    let settingsPromise = this.settingsService.get(Settings.BackgroundOptions);
    let tokenPromise = this.settingsService.get(Settings.SecurityToken);

    let allPromises = Promise.all([urlPromise, settingsPromise, tokenPromise]);

    allPromises.then((values) => {

      let trackingUrl = values[0];
      let bgOptions = values[1];
      let token = values[2];

      if (!bgOptions) {
        this.logger.info('no bgOptions settings');

        bgOptions = this.defaultBgOptions;
        this.settingsService.set(Settings.BackgroundOptions, bgOptions);
      }

      this.logger.info('starting with location tracking options:')
      this.logger.info(bgOptions);

      let onDevice = this.platform.is('cordova');

      if (onDevice) {
        this.platform.ready().then(() => {
          bgOptions.url = `${trackingUrl}/api/devices/${device.uuid}/points`;
          bgOptions.httpHeaders = { Authorization: 'Bearer ' + token }
          this.start(bgOptions);

          this.logger.info('start done!');
          startFinished();
        });
      } else {
        this.logger.info('start done!');
        startFinished();
      }
    });

    return this.startPromise;
  }

  stopTracking() {
    this.logger.info('tracking stopped');

    if (!this.isTracking) {
      return;
    }

    this.isTracking = false;

    let onDevice = this.platform.is('cordova');

    onDevice && this.platform.ready().then(() => {
      this.stop();
    })
  }

  toggleTracking() {
    if (this.isTracking) {
      this.stopTracking();
    }
    else {
      this.startTracking();
    }
  }

  private start(configuration: any) {
    backgroundGeolocation.configure(
      (location) => this.successCallback(location),
      () => this.failureCallback(),
      configuration
    );

    backgroundGeolocation.start();
  }

  private stop() {
    backgroundGeolocation.stop();
  }

  private successCallback(location) {
    this.logger.info(`background location callback (${location.latitude}, ${location.longitude})`);

    this.locationsSubject.next(location);
    backgroundGeolocation.finish();
  }

  private failureCallback() {
    this.logger.info('background location error');
  }
}

