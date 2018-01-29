import { NgModule } from '@angular/core';
import { Routes, RouterModule, CanActivate } from '@angular/router';

import { UserListComponent } from './user-list/user-list.component';
import { UserEditorComponent } from './user-editor/user-editor.component';

const routes: Routes = [
  {
    path: 'users/new',
    component: UserEditorComponent
  },
  {
    path: 'users',
    component: UserListComponent
  },
  {
    path: 'users/:id',
    component: UserEditorComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }

export const usersRoutedComponents = [
  UserListComponent,
  UserEditorComponent
];
