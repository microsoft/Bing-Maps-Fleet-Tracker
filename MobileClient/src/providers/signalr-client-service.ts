import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { SettingsService, Settings } from './settings-service';
import { LocalNotificationProvider } from './local-notification';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Observable } from 'rxjs/Observable';
import {DispatchingParameters} from '../shared/dispatching-parameters'




@Injectable()
export class SignalrClientServiceProvider {

  private paramsSubject: ReplaySubject<any> = new ReplaySubject<any>();
  params: Observable<any> = this.paramsSubject.asObservable();

  private hubConnection: HubConnection;

  constructor(public http: Http,
    private settingsService: SettingsService,
    private LocalNotification : LocalNotificationProvider
  ) {
    
  }

  public connect() {

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://fleettrackermobileclient.azurewebsites.net/dispatchingClient'
        ,
        {
          accessTokenFactory: () => this.settingsService.get(Settings.SecurityToken)
        }
      )
      .configureLogging(signalR.LogLevel.Information)
      .build();


    this.hubConnection
      .start()
      .then(() => console.log('signalr hub Connection started!'))
      .catch(err => console.log('Error while establishing hub connection !' + err));

        this.hubConnection.on('DispatchParameters', (params : DispatchingParameters) => {
        this.LocalNotification.sendNotification("Dispatching Service Began");
        this.paramsSubject.next(params);
     });
  }

}
