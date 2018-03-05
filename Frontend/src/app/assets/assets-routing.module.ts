// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';
import { Routes, RouterModule, CanActivate } from '@angular/router';

import { AssetListComponent } from './asset-list/asset-list.component';
import { AssetEditorComponent } from './asset-editor/asset-editor.component';

const routes: Routes = [
  {
    path: 'assets/new', component: AssetEditorComponent
  },
  {
    path: 'assets/:id', component: AssetEditorComponent
  },
  {
    path: 'assets', component: AssetListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AssetsRoutingModule { }

export const assetsRoutedComponents = [
  AssetListComponent,
  AssetEditorComponent
];
