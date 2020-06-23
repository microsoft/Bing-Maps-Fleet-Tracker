// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { UsersRoutingModule, usersRoutedComponents } from './users-routing.module';
import { InstrumentationApprovalComponent } from './instrumentation-approval/instrumentation-approval.component';
import { LoginComponent } from './login/login.component';
import { UserService } from './user.service';
import { AuthService } from './auth.service';
import { UsersInfoDialogComponent } from './users-info-dialog/users-info-dialog.component';

@NgModule({
  declarations: [
    usersRoutedComponents,
    LoginComponent,
    InstrumentationApprovalComponent,
    UsersInfoDialogComponent
  ],
  entryComponents: [
    InstrumentationApprovalComponent,
    UsersInfoDialogComponent
  ],
  imports: [
    UsersRoutingModule,
    SharedModule
  ],
  providers: [UserService, AuthService],
  exports: [
    LoginComponent,
  ]
})
export class UsersModule { }
