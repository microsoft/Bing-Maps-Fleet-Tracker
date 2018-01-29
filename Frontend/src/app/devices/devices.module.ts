
import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { DevicesRoutingModule, routedComponents } from './devices-routing.module';
import { DeviceService } from './device.service';
import { DeviceRegisterComponent } from './device-register/device-register.component';


@NgModule({
  declarations: [
    routedComponents,
    DeviceRegisterComponent
  ],
  entryComponents: [
    DeviceRegisterComponent
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
