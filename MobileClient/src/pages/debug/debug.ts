import { Component } from '@angular/core';
import { NavController, NavParams, ToastController } from 'ionic-angular';
import { Logger } from 'angular2-logger/core';
import { Platform } from 'ionic-angular';

import { SettingsService, Settings } from '../../providers/settings-service';
import { BackgroundTrackerService } from '../../providers/background-tracker-service';
import { LocationDetailsPage } from '../location-details/location-details';

declare var device;

/*
  Generated class for the Debug page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-debug',
  templateUrl: 'debug.html'
})
export class DebugPage {

  private uuid: Promise<string>;
  private locations: any[] = [];
  showAndroid: boolean = false;
  showIos: boolean = false;

  bgInitialOptions;
  bgOptions = {};
  deviceToken = '';

  constructor(
    public navCtrl: NavController,
    public navParams: NavParams,
    public platform: Platform,
    private backgroundTrackerService: BackgroundTrackerService,
    private settingsService: SettingsService,
    private logger: Logger,
    private toastController: ToastController) {

    this.backgroundTrackerService.locations.subscribe((location) => this.locations.push(location));
    this.settingsService.get(Settings.SecurityToken).then(val => {
      this.deviceToken = val;
    });
  }

  showLocationDetails(location) {
    this.navCtrl.push(LocationDetailsPage, { location: location });
  }


  toInt(str): number {
    return parseInt(str);
  }

  ngOnInit() {
    this.platform.ready().then(() => this.platformReady());
    this.getOptions();
  }

  platformReady() {
    this.showAndroid = this.platform.is('android') || this.platform.is('core');
    this.showIos = this.platform.is('ios') || this.platform.is('core');
  }

  getOptions() {
    this.settingsService.get(Settings.BackgroundOptions).then((val) => {
      this.bgInitialOptions = val;
      this.reset();
    });
  }

  recycleService() {
    this.backgroundTrackerService.stopTracking();
    this.backgroundTrackerService.startTracking().then(() => this.getOptions());

    this.toastController.create({
      message: 'Restarting tracking service',
      duration: 3000,
      position: 'top'
    }).present();
  }

  reset() {
    Object.assign(this.bgOptions, this.bgInitialOptions);
  }

  save() {
    this.settingsService.set(Settings.BackgroundOptions, this.bgOptions).then(() => {
      this.recycleService();
    });
  }

  defaults() {
    this.settingsService.remove(Settings.BackgroundOptions).then(() => {
      this.recycleService();
    });
  }

  shallowCompare(obj1, obj2) {
    for (let p in obj1) {
      if (obj1.hasOwnProperty(p) && obj1[p] !== obj2[p]) {
        return false;
      }
    }

    for (let p in obj2) {
      if (obj2.hasOwnProperty(p) && !obj1.hasOwnProperty(p)) {
        return false;
      }
    }

    return true;
  }
}
