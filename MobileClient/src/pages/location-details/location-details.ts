// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component } from '@angular/core';
import { NavController, NavParams } from 'ionic-angular';
import { Logger } from 'angular2-logger/core';

/*
  Generated class for the LocationDetails page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-location-details',
  templateUrl: 'location-details.html'
})
export class LocationDetailsPage {
  loc;

  constructor(public navCtrl: NavController, public navParams: NavParams, private logger: Logger) {
    this.loc = navParams.get('location');
  }

  ionViewDidLoad() {
    this.logger.info('ionViewDidLoad LocationDetailsPage');
  }

}
