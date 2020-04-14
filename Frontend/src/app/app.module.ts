// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';

import { AssetsModule } from './assets/assets.module';
import { CoreModule } from './core/core.module';
import { DevicesModule } from './devices/devices.module';
import { DispatchingModule } from './dispatching/dispatching.module';
import { GeofencesModule } from './geofences/geofences.module';
import { LocationsModule } from './locations/locations.module';
import { MapsModule } from './maps/maps.module';
import { ReportsModule } from './reports/reports.module';
import { SharedModule } from './shared/shared.module';
import { ToasterModule } from 'angular2-toaster';
import { UsersModule } from './users/users.module';

export const routing = RouterModule.forRoot([
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '/assets'
  }
]);

@NgModule({
  bootstrap: [AppComponent],
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    ToasterModule,
    AssetsModule,
    CoreModule,
    DevicesModule,
    DispatchingModule,
    LocationsModule,
    MapsModule,
    SharedModule,
    ReportsModule,
    UsersModule,
    GeofencesModule,
    routing
  ]
})
export class AppModule { }
