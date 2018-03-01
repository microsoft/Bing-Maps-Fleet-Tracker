// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LocationListComponent } from './location-list/location-list.component';
import { LocationEditorComponent } from './location-editor/location-editor.component';

const routes: Routes = [
  {
    path: 'locations/new',
    component: LocationEditorComponent
  },
  {
    path: 'locations',
    component: LocationListComponent
  },
  {
    path: 'locations/:id',
    component: LocationEditorComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LocationsRoutingModule { }

export const locationsRoutedComponents = [
  LocationListComponent,
  LocationEditorComponent
];
