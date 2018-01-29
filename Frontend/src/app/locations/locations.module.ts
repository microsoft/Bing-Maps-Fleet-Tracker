import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { LocationsRoutingModule, locationsRoutedComponents } from './locations-routing.module';
import { LocationService } from './location.service';
import { LocationEditorComponent } from './location-editor/location-editor.component';

@NgModule({
  declarations: [
    locationsRoutedComponents
  ],
  imports: [
    SharedModule,
    LocationsRoutingModule
  ],
  providers: [
    LocationService
  ]
})
export class LocationsModule { }
