// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { AssetsRoutingModule, assetsRoutedComponents } from './assets-routing.module';
import { AssetService } from './asset.service';
import { MatDialogModule } from '@angular/material';
import { AssetInfoDialogComponent } from './asset-info-dialog/asset-info-dialog.component';


@NgModule({
  declarations: [
    assetsRoutedComponents,
    AssetInfoDialogComponent
  ],
  imports: [
    AssetsRoutingModule,
    SharedModule,
    MatDialogModule
  ],
  entryComponents: [
    AssetInfoDialogComponent
  ],
  providers: [
    AssetService
  ]
})
export class AssetsModule { }
