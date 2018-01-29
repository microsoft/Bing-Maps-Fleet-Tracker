import { Component } from '@angular/core';
import { NavController, NavParams } from 'ionic-angular';
import { Platform, ToastController } from 'ionic-angular';
import { Logger } from 'angular2-logger/core';

import { SettingsService, Settings } from '../../providers/settings-service';
import { BackgroundTrackerService } from '../../providers/background-tracker-service';
import { SignallingService } from '../../providers/signalling-service';

declare var device;

@Component({
  selector: 'page-settings',
  templateUrl: 'settings.html'
})
export class SettingsPage {

  trackingServiceRunning: boolean;
  backendUrl: string;
  uuid: Promise<any>;
  private clicksToSettings = 5;

  constructor(
    public navCtrl: NavController,
    public navParams: NavParams,
    private logger: Logger,
    private platform: Platform,
    private settingsService: SettingsService,
    private signallingService: SignallingService,
    private backgroundTrackerService: BackgroundTrackerService,
    private toastController: ToastController) {
  }

  ngOnInit() {
    this.trackingServiceRunning = this.backgroundTrackerService.isTracking;

    this.settingsService.get(Settings.BackendUrl).then(val => {
      this.backendUrl = val;
    });

    this.uuid = this.platform.ready().then(() => {
      return (typeof (device) !== 'undefined' ? device.uuid : 'running in browser');
    });
  }

  debugClick() {
    if (this.clicksToSettings > -1) {
      this.clicksToSettings--;
      if (this.clicksToSettings == 0) {
        this.signallingService.showDebugTab();

        this.toastController.create({
          message: 'Showing debug tab',
          duration: 3000,
          position: 'top'
        }).present();
      }
    }
  }

  startService() {
    this.backgroundTrackerService.startTracking();

    this.toastController.create({
      message: 'Starting tracking service',
      duration: 3000,
      position: 'top'
    }).present();

    this.trackingServiceRunning = this.backgroundTrackerService.isTracking;
  }

  stopService() {
    this.backgroundTrackerService.stopTracking();

    this.toastController.create({
      message: 'Stopping tracking service',
      duration: 3000,
      position: 'top'
    }).present();

    this.trackingServiceRunning = this.backgroundTrackerService.isTracking;
  }

  recycleService() {
    this.backgroundTrackerService.stopTracking();
    this.backgroundTrackerService.startTracking();

    this.toastController.create({
      message: 'Restarting tracking service',
      duration: 3000,
      position: 'top'
    }).present();

    this.trackingServiceRunning = this.backgroundTrackerService.isTracking;
  }

  clearUrlSettings() {
    Promise.all([
      this.settingsService.remove(Settings.SecurityToken),
      this.settingsService.remove(Settings.BackendUrl)])
      .then(() => location.reload());
  }
}
