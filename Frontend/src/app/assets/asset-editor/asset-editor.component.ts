// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ToasterService } from 'angular2-toaster';

import { Asset, AssetType } from '../asset';
import { Roles } from '../../shared/role';
import { AssetProperties } from '../../shared/asset-properties';
import { AssetService } from '../asset.service';
import { Device } from '../../devices/device';
import { DeviceService } from '../../devices/device.service';
import 'rxjs/add/operator/takeWhile';

@Component({
  selector: 'app-asset-editor-dialog',
  templateUrl: './asset-editor.component.html',
  styleUrls: ['./asset-editor.component.css']
})

export class AssetEditorComponent implements OnInit, OnDestroy {
  asset: Asset;
  devices: Device[];
  isEditable: boolean;
  assetProperties: AssetProperties;
  AssetType = AssetType;
  Roles = Roles;
  private isAlive: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private assetService: AssetService,
    private deviceService: DeviceService,
    private toasterService: ToasterService) {
    this.asset = new Asset();
    this.asset.assetType = AssetType.Car;
    this.assetProperties = new AssetProperties();
  }

  ngOnInit() {
    this.isAlive = true;

    this.route.params
      .takeWhile(() => this.isAlive)
      .subscribe(params => {
        const id = params['id'];
        if (id) {
          this.isEditable = true;
          this.assetService.getAsset(id)
            .takeWhile(() => this.isAlive)
            .subscribe(asset => {
              this.asset = asset;
              this.assetProperties = this.asset.assetProperties;
            });
        }
      });

    this.deviceService.getDevices()
      .takeWhile(() => this.isAlive)
      .subscribe(devices => this.devices = devices);
  }

  ngOnDestroy() {
    this.isAlive = false;
  }

  submit() {
    if (this.asset.assetType === AssetType.Truck) {
      this.asset.assetProperties = this.assetProperties;
    }

    if (this.isEditable) {
      this.assetService.updateAsset(this.asset)
        .subscribe(() => this.router.navigate(['/assets']));
    } else {
      this.assetService.addAsset(this.asset)
        .subscribe(() => this.router.navigate(['/assets']), error => {
          this.toasterService.pop('error', 'Invalid Input', 'Asset ID already exists');
        });
    }
  }

  deleteAsset() {
    this.assetService.deleteAsset(this.asset)
      .subscribe(() => this.router.navigate(['/assets']));
  }
}
