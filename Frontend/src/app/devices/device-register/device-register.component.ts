import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  templateUrl: './device-register.component.html',
  styleUrls: ['./device-register.component.css']
})
export class DeviceRegisterComponent {
  qrCodeEndpoint = '';

  constructor( @Inject(MAT_DIALOG_DATA) public data: any) {
    this.qrCodeEndpoint = data;
  }
}
