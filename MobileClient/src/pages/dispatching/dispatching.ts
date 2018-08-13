// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component } from '@angular/core';
import { NavController, NavParams, DateTime } from 'ionic-angular';
import { Platform, ToastController } from 'ionic-angular';
import { Logger } from 'angular2-logger/core';
import { SignalrClientServiceProvider } from '../../providers/signalr-client-service';
import { MapHostService } from '../../providers/map-host-service';
import { DispatchingParameters } from '../../shared/dispatching-parameters';

declare var device;

@Component({
  selector: 'page-dispatching',
  templateUrl: 'dispatching.html',
})
export class DispatchingPage {

  private dispatches: DispatchingParameters[];
  private noDispatches: boolean;
  private optimizeValue: string[];
  
  constructor(
    public navCtrl: NavController,
    public navParams: NavParams,
    private logger: Logger,
    private platform: Platform,
    private signalr: SignalrClientServiceProvider,
    private maphost: MapHostService
    ) {
      this.optimizeValue = ["Time" , "Time Considering Traffic"];
      this.dispatches = [];
  }

  ngOnInit() {

    this.noDispatches = true;
    this.signalr.params.subscribe((params) => {
      this.noDispatches = false;
      this.dispatches.push(params);
    });
  }

  Navigate(params) {
     this.maphost.getDirections(params);
  }

  stopNavigate(params){
    this.maphost.clearDirections(params);
  }
}

