// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Observable } from 'rxjs/Observable';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';

import { VersionInfo } from '../version-info';
import { SubscriptionKey } from '../subscription-key';
import { SettingsService } from '../settings.service';
import { InstrumentationApprovalComponent } from '../../users/instrumentation-approval/instrumentation-approval.component';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {

  subkeys: Observable<SubscriptionKey[]>;
  instrumentationState = null;
  versionInfo = new VersionInfo();

  constructor(private settingsService: SettingsService, private dialog: MatDialog) {
  }

  ngOnInit() {
    this.subkeys = this.settingsService.getSubscriptionKeys();
    this.settingsService.getVersionInfo().subscribe(v => this.versionInfo = v);
    this.settingsService.getInstrumentationApproval().subscribe(a => this.instrumentationState = a);
  }

  toggleInstrumentation() {
    if (!this.instrumentationState) {
      this.dialog.open(InstrumentationApprovalComponent, {
        width: '70%',
      }).afterClosed()
        .subscribe(res => {
          if (res != null) {
            this.settingsService.setInstrumentationApproval(res);
            this.instrumentationState = res;
          }
        });
    } else {
      this.settingsService.setInstrumentationApproval(false)
        .subscribe(() => this.instrumentationState = false);
    }
  }
}
