// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ToasterService } from 'angular2-toaster';
import { Toast } from 'angular2-toaster';
import { MatDialog } from '@angular/material/dialog';

import { User } from '../../shared/user';
import { Role, Roles } from '../../shared/role';

import { AuthService } from '../auth.service';
import { ReportService } from '../../reports/report.service';
import { SettingsService } from '../../core/settings.service';
import { InstrumentationApprovalComponent } from '../instrumentation-approval/instrumentation-approval.component';

import { skipWhile, take } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {

  loggedInUser: User = null;
  link: string = this.authService.getLoginUrl();
  userSubscription: Subscription;

  constructor(
    private authService: AuthService,
    private reportService: ReportService,
    private settingsService: SettingsService,
    private toasterService: ToasterService,
    private dialog: MatDialog
  ) {
  }

  ngOnInit() {
    this.userSubscription = this.authService.loginOrGetUser()
      .pipe(skipWhile(v => v === null))
      .subscribe(value => {
        this.loggedInUser = value;
        this.link = this.authService.getLogoutUrl();

        if (value.role.name === Roles[Roles.Administrator] ||
          value.role.name === Roles[Roles.Owner]) {
          this.reportService.getAllMetrics().subscribe();
        }

        if (value.role.name === Roles[Roles.Owner]) {
          this.settingsService.getVersionInfo()
            .subscribe(result => {
              if (result.updateRequired) {
                const toast: Toast = {
                  type: 'info',
                  title: 'Update Available',
                  body: `A new version "${result.latestVersionName}" is available, click here to upgrade to the latest version.`,
                  clickHandler: (t, isClosed): boolean => {
                    if (!isClosed) {
                      window.location.href = result.updateUrl;
                    }
                    return true;
                  },
                  showCloseButton: true,
                  timeout: 10000,
                };
                this.toasterService.pop(toast);
              }
            }
            );

          this.settingsService.getInstrumentationApproval().pipe(take(1)).subscribe(val => {
            if (val == null) {
              this.dialog.open(InstrumentationApprovalComponent, {
                width: '70%',
              }).afterClosed().subscribe(res => {
                if (res != null) {
                  this.settingsService.setInstrumentationApproval(res);
                }
              });
            }
          });
        }
      });
  }

  ngOnDestroy() {
    this.userSubscription.unsubscribe();
  }
}