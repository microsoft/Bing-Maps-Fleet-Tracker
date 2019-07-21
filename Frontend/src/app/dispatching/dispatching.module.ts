// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../shared/shared.module';
import { DispatchingRoutingModule , routedComponenets} from './dispatching-routing.module';
import { DispatchingService } from './dispatching.service';
import { DialogService } from './dialog.service';
import { DispatchingShowComponent } from './dispatching-show/dispatching-show.component';
import { LocationDialogComponent } from './location-dialog/location-dialog.component';
import { MatMenuModule, MatTabsModule} from '@angular/material';
import { DispatchingInfoDialogComponent } from './dispatching-info-dialog/dispatching-info-dialog.component';

@NgModule({
  declarations: [
    routedComponenets,
    DispatchingShowComponent,
    LocationDialogComponent,
    DispatchingInfoDialogComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    DispatchingRoutingModule,
    MatMenuModule,
    MatTabsModule
  ],
  providers: [
    DispatchingService,
    DialogService
  ],
  entryComponents: [
    LocationDialogComponent,
    DispatchingInfoDialogComponent
  ],
  exports: [
    LocationDialogComponent
  ]
})
export class DispatchingModule { }
