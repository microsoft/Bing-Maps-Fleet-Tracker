// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { LocationsRoutingModule, locationsRoutedComponents } from './locations-routing.module';
import { LocationService } from './location.service';
import { LocationEditorComponent } from './location-editor/location-editor.component';
import { LocationsInfoDialogComponent } from './locations-info-dialog/locations-info-dialog.component';

@NgModule({
  declarations: [
    locationsRoutedComponents,
    LocationsInfoDialogComponent
  ],
  imports: [
    SharedModule,
    LocationsRoutingModule
  ],
  entryComponents: [
    LocationsInfoDialogComponent
  ],
  providers: [
    LocationService
  ]
})
export class LocationsModule { }
