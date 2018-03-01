// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component } from '@angular/core';
import { NavController, NavParams, ViewController } from 'ionic-angular';
import { Platform } from 'ionic-angular';
import { Logger } from 'angular2-logger/core';

import { RegistrationService } from '../../providers/registration-service';

declare var cordova;

/*
  Generated class for the Registration page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-registration',
  templateUrl: 'registration.html'
})
export class RegistrationPage {

  onDevice;

  constructor(
    public navCtrl: NavController,
    public navParams: NavParams,
    private viewCtrl: ViewController,
    private logger: Logger,
    private platform: Platform,
    private registrationSvc: RegistrationService) {

    this.onDevice = this.platform.is('cordova');
  }

  scan() {
    this.platform.ready().then(() => {
      cordova.plugins.barcodeScanner.scan((result) => {
        if (!result.cancelled) {
          let deviceData = JSON.parse(result.text);
          this.logger.info(`Registering with: ${deviceData.RegistrationUrl}`);

          this.registrationSvc.register(deviceData.RegistrationUrl, deviceData.RegistrationToken,
            (value) => {
              this.logger.info('Registeration succeeded!');
              this.viewCtrl.dismiss({ baseUrl: deviceData.BaseUrl, deviceToken: value });
            },
            (error) => {
              this.logger.error(`Registeration failed (${error})!`);
              alert(error);
            });
        }
      });
    });
  }

}
