// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { DevicesRoutingModule, routedComponents } from './devices-routing.module';
import { DeviceService } from './device.service';
import { DeviceRegisterComponent } from './device-register/device-register.component';
import { DevicesInfoDialogComponent } from './devices-info-dialog/devices-info-dialog.component';


@NgModule({
  declarations: [
    routedComponents,
    DeviceRegisterComponent,
    DevicesInfoDialogComponent
  ],
  entryComponents: [
    DeviceRegisterComponent,
    DevicesInfoDialogComponent
  ],
  imports: [
    DevicesRoutingModule,
    SharedModule
  ],
  providers: [
    DeviceService
  ]
})
export class DevicesModule { }
