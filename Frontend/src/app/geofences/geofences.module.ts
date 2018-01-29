
import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { GeofencesRoutingModule, geofencesRoutedComponents } from './geofences-routing.module';
import { GeofenceService } from './geofence.service';

@NgModule({
  declarations: [
    geofencesRoutedComponents,
  ],
  imports: [
    GeofencesRoutingModule,
    SharedModule
  ],
  providers: [GeofenceService],
  exports: []
})
export class GeofencesModule { }
