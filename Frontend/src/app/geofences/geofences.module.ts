// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { GeofencesRoutingModule, geofencesRoutedComponents } from './geofences-routing.module';
import { GeofenceService } from './geofence.service';
import { GeofencesInfoDialogComponent } from './geofences-info-dialog/geofences-info-dialog.component';

@NgModule({
  declarations: [
    geofencesRoutedComponents,
    GeofencesInfoDialogComponent,
  ],
  imports: [
    GeofencesRoutingModule,
    SharedModule
  ],
  entryComponents: [
    GeofencesInfoDialogComponent
  ],
  providers: [GeofenceService],
  exports: []
})
export class GeofencesModule { }
