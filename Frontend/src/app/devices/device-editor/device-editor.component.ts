// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { ToasterService } from 'angular2-toaster';
// import { HubConnection } from '@aspnet/signalr-client';
import * as signalR from "@aspnet/signalr";
import { UUID } from 'angular2-uuid';

import { Roles } from '../../shared/role';
import { Asset } from '../../assets/asset';
import { AssetService } from '../../assets/asset.service';
import { Device } from '../device';
import { DeviceService } from '../device.service';
import { DeviceRegisterComponent } from '../device-register/device-register.component';





@Component({
  selector: 'app-device-editor-dialog',
  templateUrl: './device-editor.component.html',
  styleUrls: ['./device-editor.component.css']
})

export class DeviceEditorComponent implements OnInit, OnDestroy {
  device: Device;
  isEditing: boolean;
  assets: Asset[];
  deviceToken = '';
  Roles = Roles;

  private routerSubscription: Subscription;
  private assetsSubscription: Subscription;
  private deviceSubscription: Subscription;
  private nonce: string;
  private hubConnection: signalR.HubConnection;
  private dialogRef: MatDialogRef<any>;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private assetService: AssetService,
    private deviceService: DeviceService,
    private toasterService: ToasterService,
    private dialog: MatDialog) {
    this.device = new Device();
    this.nonce = UUID.UUID();
  }

  ngOnInit() {
    this.routerSubscription = this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.isEditing = true;
        this.deviceSubscription = this.deviceService.getDevice(id).subscribe(device => this.device = device);
        this.deviceService.getToken(id).take(1).subscribe(t => this.deviceToken = t);
      } else {
        // Work around for the fact that angular doesnt see this in change detection
        setTimeout(() => this.openDialog(), 0);
      }
    });

    this.assetsSubscription = this.assetService.getAssets()
      .subscribe(assets => {
        // Due to a bug in mat-select when bound to async Observable
        // we have to set the value of assets only once or else it will
        // throw an exception when the user clicks on the drop down control.
        if (!this.assets && assets.length) {
          this.assets = assets;
          // this.selectedAssetId = this.device && this.device.assetId;
        }
      });

    this.hubConnection =  new signalR.HubConnectionBuilder()
      .withUrl(this.deviceService.getDeviceAdditionNotificationUrl())
      .build();

    this.hubConnection.on('DeviceAdded', (data: any) => {
      if (data === this.nonce) {
        this.dialogRef.close();
        this.router.navigate(['/devices']);
      }
    });

    this.hubConnection.start();
  }

  ngOnDestroy() {
    this.routerSubscription.unsubscribe();
    this.assetsSubscription.unsubscribe();

    if (this.deviceSubscription && !this.deviceSubscription.closed) {
      this.deviceSubscription.unsubscribe();
    }

    this.hubConnection.stop();
  }

  submit() {
    if (this.isEditing) {
      this.deviceService.updateDevice(this.device)
        .subscribe(() => this.router.navigate(['/devices']));
    } else {
      this.deviceService.addDevice(this.device)
        .subscribe(() => this.router.navigate(['/devices']), error => {
          this.toasterService.pop('error', 'Invalid Input', error._body);
        });
    }
  }

  openDialog() {
    this.dialogRef = this.dialog.open(DeviceRegisterComponent, {
      height: '70%',
      width: '70%',
      data: this.deviceService.getProvisioningQrCodeUrl(this.nonce)
    });
  }

  deleteDevice() {
    this.deviceService.deleteDevice(this.device)
      .subscribe(() => this.router.navigate(['/devices']));
  }
}
