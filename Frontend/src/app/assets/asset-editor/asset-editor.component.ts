// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { ToasterService } from 'angular2-toaster';

import { Asset, AssetType } from '../asset';
import { AssetProperties } from '../../shared/asset-properties';
import { AssetService } from '../asset.service';

@Component({
  selector: 'app-asset-editor-dialog',
  templateUrl: './asset-editor.component.html',
  styleUrls: ['./asset-editor.component.css']
})

export class AssetEditorComponent implements OnInit, OnDestroy {
  asset: Asset;
  isEditable: boolean;
  assetProperties: AssetProperties;
  AssetType = AssetType;
  private routerSubscription: Subscription;
  private assetSubscription: Subscription;

  constructor(private route: ActivatedRoute,
    private router: Router,
    private assetService: AssetService,
    private toasterService: ToasterService) {
    this.asset = new Asset();
    this.asset.assetType = AssetType.Car;
    this.assetProperties = new AssetProperties();
  }

  ngOnInit() {
    this.routerSubscription = this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.isEditable = true;
        this.assetSubscription = this.assetService.getAsset(id).subscribe(asset => this.asset = asset);
      }
    });
  }

  ngOnDestroy() {
    this.routerSubscription.unsubscribe();

    if (this.assetSubscription && !this.assetSubscription.closed) {
      this.assetSubscription.unsubscribe();
    }
  }

  submit() {
    if (this.asset.assetType === AssetType.Truck) {
      this.asset.assetProperties = this.assetProperties;
    }

    this.assetService.addAsset(this.asset)
      .subscribe(() => this.router.navigate(['/assets']), error => {
        this.toasterService.pop('error', 'Invalid Input', 'Asset ID already exists');
      });
  }
}
