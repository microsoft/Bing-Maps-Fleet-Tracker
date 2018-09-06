// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { LocalNotifications } from '@ionic-native/local-notifications';

@Injectable()
export class LocalNotificationProvider {

  constructor(public http: Http,
    private localNotifications: LocalNotifications) {
  }

  public sendNotification(message : string){
    this.localNotifications.schedule({
      id: 1,
      text: message,
      autoClear: true,
      launch: true,
      sticky: false,      
    });
  }
}
