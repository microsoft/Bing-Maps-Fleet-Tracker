// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../shared/shared.module';
import { DispatchingRoutingModule, routedComponenets } from './dispatching-routing.module';
import { DispatchingService } from './dispatching.service';
import { DialogService } from './dialog.service';
import { DispatchingShowComponent } from './dispatching-show/dispatching-show.component';
import { LocationDialogComponent } from './location-dialog/location-dialog.component';
import { MatMenuModule} from '@angular/material/menu';

@NgModule({
  declarations: [
    routedComponenets,
    DispatchingShowComponent,
    LocationDialogComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    DispatchingRoutingModule,
    MatMenuModule
  ],
  providers: [
    DispatchingService,
    DialogService
  ],
  entryComponents: [
    LocationDialogComponent
  ],
  exports: [
    LocationDialogComponent
  ]
})
export class DispatchingModule { }
