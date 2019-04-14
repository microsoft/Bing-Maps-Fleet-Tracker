// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, NgZone, ViewChild, ElementRef } from '@angular/core';
import { NavController, ModalController, Platform, ToastController } from 'ionic-angular';
import { Network } from 'ionic-native';

import { Logger } from 'angular2-logger/core';

import { BackgroundTrackerService } from '../../providers/background-tracker-service';
import { MapHostService } from '../../providers/map-host-service';

import { RegistrationPage } from '../registration/registration';
import { SettingsService, Settings } from '../../providers/settings-service';

declare var navigator: any; 
declare var Connection: any;


@Component({
  selector: 'page-home',
  templateUrl: 'home.html',
  providers: [
    MapHostService
  ]
})
export class HomePage {

  @ViewChild('map') mapElement: ElementRef;

  onDevice: boolean;
  isOnline: boolean = false;

  connectSub;
  disconnectSub;

  mapHostInitialized = false;
  
  constructor(
    public navCtrl: NavController, 
    private logger: Logger,
    private platform: Platform,
    private ngZone: NgZone,
    private backgroundTrackerService: BackgroundTrackerService,
    private mapHostService: MapHostService,
    public modalCtrl: ModalController,
    private settingsSrvc: SettingsService,
    private toastController: ToastController) {  

      this.onDevice = this.platform.is('cordova');
      this.platform.ready().then(() => {
        this.isOnline = !this.onDevice || navigator.connection.type !== 'none';
      });
    }

    get isTracking() {
      return this.backgroundTrackerService.isTracking;
    }

    ngOnInit() {
      this.connectSub = Network.onConnect().subscribe(() => this.ngZone.run(() => this.connected()));
      this.disconnectSub = Network.onDisconnect().subscribe(() => this.ngZone.run(() => this.disconnected()));

      this.settingsSrvc.get(Settings.BackendUrl).then((val) => {
        if (!val) {
          let registration = this.modalCtrl.create(RegistrationPage);
          registration.onDidDismiss(data => {
            if(!data){
              navigator.app.exitApp();
            }

            Promise.all([
              this.settingsSrvc.set(Settings.BackendUrl, data.baseUrl),
              this.settingsSrvc.set(Settings.SecurityToken, data.deviceToken),
              ]).then(() => {this.continueInitialization();});
          });
          registration.present();
        }
        else {
          this.continueInitialization();
        }
      });
    }

    continueInitialization() {
      this.backgroundTrackerService.startTracking().then(() => {
        if(!this.backgroundTrackerService.isTracking){
          this.toastController.create({
            message: 'Your Locations Settings is set to Off \n Please Enable Location to use this app',
            duration: 5000,
            position: 'middle'
          }).present();
        }
      });

      this.mapHostInitialized = true;
      this.mapHostService.initialize(this.mapElement.nativeElement);
    }

    ngOnDestroy() {
      this.connectSub.unsubscribe();
      this.disconnectSub.unsubscribe();

      if (this.mapHostInitialized) {
        this.mapHostService.uninitialize();
      }
    }

    connected() {
      this.logger.info('connected');
      this.isOnline = true;
    }

    disconnected() {
      this.logger.info('disconnected');
      this.isOnline = false;
    }

    toggleTracking() {
      this.backgroundTrackerService.toggleTracking();
    }

    showLocation() {
      this.mapHostService.showLocation();
    }

    togglePins() {
      this.mapHostService.togglePins();
    }}
