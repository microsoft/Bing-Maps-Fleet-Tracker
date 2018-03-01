// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Platform } from 'ionic-angular';
import 'rxjs/add/operator/map';

import { Logger } from 'angular2-logger/core';

declare var cordova;
declare var device;

@Injectable()
export class RegistrationService {

  constructor(
    private logger: Logger,
    private platform: Platform,
    private http: Http,
  ) {
  }

  register(url, registrationToken, success, failure) {
    this.platform.ready().then(() => this.registerInternal(url, registrationToken, success, failure));
  }

  private registerInternal(url, registrationToken, success, failure) {

    let deviceName = cordova.plugins.deviceName && cordova.plugins.deviceName.name;

    let registrationData = {
      Id: device.uuid,
      Name: deviceName,
      Model: device.model,
      OperatingSystem: device.platform,
      Version: device.version
    };

    let headers = new Headers();
    headers.append('Authorization', 'Bearer ' + registrationToken);

    this.http.post(url, registrationData, { headers: headers })
      .subscribe(data => success(data.json()), error => failure(error));
  }

}
