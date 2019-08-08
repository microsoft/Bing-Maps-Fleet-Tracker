// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  templateUrl: './device-register.component.html',
  styleUrls: ['./device-register.component.css']
})
export class DeviceRegisterComponent {
  qrCodeEndpoint = '';

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
    this.qrCodeEndpoint = data;
  }
}
