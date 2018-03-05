// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { BingMapsService } from './bing-maps.service';
import { MapsComponent } from './maps/maps.component';
import { MapsService } from './maps.service';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [
    MapsComponent
  ],
  imports: [
      SharedModule
  ],
  providers: [
    BingMapsService, MapsService
  ],
  exports: [
      MapsComponent
  ]
})
export class MapsModule { }
