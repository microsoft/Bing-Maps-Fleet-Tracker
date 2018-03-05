// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { AssetsRoutingModule, assetsRoutedComponents } from './assets-routing.module';
import { AssetService } from './asset.service';


@NgModule({
  declarations: [
    assetsRoutedComponents
  ],
  imports: [
    AssetsRoutingModule,
    SharedModule
  ],
  providers: [
    AssetService
  ]
})
export class AssetsModule { }
