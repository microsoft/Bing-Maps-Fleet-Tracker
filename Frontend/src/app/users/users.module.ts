
import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { UsersRoutingModule, usersRoutedComponents } from './users-routing.module';
import { InstrumentationApprovalComponent } from './instrumentation-approval/instrumentation-approval.component';
import { LoginComponent } from './login/login.component';
import { UserService } from './user.service';
import { AuthService } from './auth.service';

@NgModule({
  declarations: [
    usersRoutedComponents,
    LoginComponent,
    InstrumentationApprovalComponent
  ],
  entryComponents: [InstrumentationApprovalComponent],
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
