// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';
import { Routes, RouterModule, CanActivate } from '@angular/router';

import { GeofenceListComponent } from './geofence-list/geofence-list.component';
import { GeofenceEditorComponent } from './geofence-editor/geofence-editor.component';

const routes: Routes = [
  {
    path: 'geofences/new',
    component: GeofenceEditorComponent
  },
  {
    path: 'geofences',
    component: GeofenceListComponent
  },
  {
    path: 'geofences/:id', component: GeofenceEditorComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GeofencesRoutingModule { }

export const geofencesRoutedComponents = [
  GeofenceListComponent,
  GeofenceEditorComponent
];
