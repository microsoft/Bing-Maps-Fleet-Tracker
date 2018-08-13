// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule, ErrorHandler } from '@angular/core';
import { IonicApp, IonicModule, IonicErrorHandler } from 'ionic-angular';
import { Storage } from '@ionic/storage';
import { LOG_LOGGER_PROVIDERS } from 'angular2-logger/core';
import { MyApp } from './app.component';
import { HomePage } from '../pages/home/home';
import { DebugPage } from '../pages/debug/debug';
import { LocationDetailsPage } from '../pages/location-details/location-details';
import { SettingsPage } from '../pages/settings/settings';
import { TabsPage } from '../pages/tabs/tabs';
import { RegistrationPage } from '../pages/registration/registration';
import { BingMapsService } from '../providers/bing-maps-service';
import { BackgroundTrackerService } from '../providers/background-tracker-service';
import { SettingsService } from '../providers/settings-service';
import { SignallingService } from '../providers/signalling-service';
import { RegistrationService } from '../providers/registration-service';
import { SignalrClientServiceProvider } from '../providers/signalr-client-service';
import {LocalNotifications} from '@ionic-native/local-notifications';
import { LocalNotificationProvider } from '../providers/local-notification';
import { DispatchingPage } from '../pages/dispatching/dispatching';
import { MapHostService } from '../providers/map-host-service';


@NgModule({
  declarations: [
    MyApp,
    HomePage,
    DebugPage,
    LocationDetailsPage,
    SettingsPage,
    TabsPage,
    RegistrationPage,
    DispatchingPage    
  ],
  imports: [
    IonicModule.forRoot(MyApp),
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    MyApp,
    HomePage,
    DebugPage,
    LocationDetailsPage,
    SettingsPage,
    TabsPage,
    RegistrationPage,
    DispatchingPage,
  ],
  providers: [
    LOG_LOGGER_PROVIDERS,
    Storage,
    BingMapsService,
    MapHostService,
    BackgroundTrackerService,
    SettingsService,
    SignallingService,
    RegistrationService,
    {provide: ErrorHandler, useClass: IonicErrorHandler},
    SignalrClientServiceProvider,
    LocalNotifications,
    LocalNotificationProvider
  ]
})
export class AppModule {}
