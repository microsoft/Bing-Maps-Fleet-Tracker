import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { LocalNotifications } from '@ionic-native/local-notifications';


@Injectable()
export class LocalNotificationProvider {

  constructor(public http: Http,
    private localNotifications: LocalNotifications) {
  }

  public sendNotification(){

    this.localNotifications.schedule({
      id: 1,
      text: 'Dispatching Service Began',
      autoClear: true,
      launch: true,
      sticky: false,      
    });

  }


}
