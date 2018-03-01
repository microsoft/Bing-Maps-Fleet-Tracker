// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DispatchingEditorComponent } from './dispatching-editor/dispatching-editor.component';
import { DispatchingShowComponent } from './dispatching-show/dispatching-show.component';

const routes: Routes = [
  {
    path: 'dispatching',
    component: DispatchingEditorComponent
  },
  {
    path: 'dispatching/show',
    component: DispatchingShowComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DispatchingRoutingModule { }

export const routedComponenets = [
  DispatchingEditorComponent,
  DispatchingShowComponent
];
