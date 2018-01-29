import { NgModule } from '@angular/core';
import { Routes, RouterModule, CanActivate } from '@angular/router';

import { DeviceListComponent } from './device-list/device-list.component';
import { DeviceEditorComponent } from './device-editor/device-editor.component';

const routes: Routes = [
  {
    path: 'devices/new',
    component: DeviceEditorComponent
  },
  {
    path: 'devices',
    component: DeviceListComponent
  },
  {
    path: 'devices/:id',
    component: DeviceEditorComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DevicesRoutingModule { }

export const routedComponents = [
  DeviceListComponent,
  DeviceEditorComponent,
];
