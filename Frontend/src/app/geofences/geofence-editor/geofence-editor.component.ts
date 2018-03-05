// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { Geofence, FenceType, AreaType } from '../../shared/geofence';
import { Point } from '../../shared/point';
import { Asset } from '../../assets/asset';
import { GeofenceService } from '../geofence.service';
import { AssetService } from '../../assets/asset.service';
import { MapsService } from '../../maps/maps.service';
import { ToasterService } from 'angular2-toaster';

@Component({
  selector: 'app-geofence-edit-dialog',
  templateUrl: './geofence-editor.component.html',
  styleUrls: ['./geofence-editor.component.css']
})

export class GeofenceEditorComponent implements OnInit, OnDestroy {
  assets: Asset[];
  geofence: Geofence;
  joinedEmails = '';
  joinedWebhooks = '';
  FenceType = FenceType;
  AreaType = AreaType;
  private assetsSubscription: Subscription;
  private geofenceSubscription: Subscription;
  private routeSubscription: Subscription;
  private resultSubscription: Subscription;
  private isEditable = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private assetService: AssetService,
    private geofenceService: GeofenceService,
    private mapsService: MapsService,
    private toasterService: ToasterService) {
    this.assets = [];
  }

  ngOnInit() {
    this.geofence = new Geofence();
    this.geofence.areaType = AreaType.Polygon;
    this.geofence.fencePolygon = new Array<Point>();

    this.routeSubscription = this.route.params.subscribe(params => {
      const id = params['id'];

      if (id) {
        this.geofenceSubscription = this.geofenceService.get(id).subscribe(geofence => {
          if (geofence != null) {
            this.geofence = geofence;
            this.joinedEmails = this.geofence.emailsToNotify.join(', ');
            this.joinedWebhooks = this.geofence.webhooksToNotify.join(', ');
            if (this.geofence.areaType === AreaType.Polygon) {
              this.mapsService.startPolygonGeofenceDraw(geofence.fencePolygon.slice(0, geofence.fencePolygon.length - 1));
            } else if (this.geofence.areaType === AreaType.Circular) {
              this.mapsService.startCircularGeofenceDraw(geofence.fenceCenter, geofence.radiusInMeters);
            }
            this.updateAssetState();
          }
        });

        this.isEditable = true;
      } else {
        this.mapsService.startPolygonGeofenceDraw(this.geofence.fencePolygon);
      }
    });

    this.assetsSubscription = this.assetService.getAssets().subscribe(assets => {
      this.assets.splice(0, this.assets.length);
      assets.forEach(a => {
        this.assets.push(a);
      });

      this.updateAssetState();
    });

    this.resultSubscription = this.mapsService.getGeodencePolygonDrawResult()
      .subscribe(result => {
        this.geofence.fencePolygon = result;
      });

    this.resultSubscription = this.mapsService.getGeodenceCircularDrawResult()
      .subscribe(result => {
        this.geofence.fenceCenter = result;
      });

    this.toasterService.pop('info', 'Draw Geofence', 'Please use the map to specify your geofence');
  }

  ngOnDestroy() {
    this.mapsService.endCurrentDraw();

    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }

    if (this.geofenceSubscription) {
      this.geofenceSubscription.unsubscribe();
    }

    if (this.assetsSubscription) {
      this.assetsSubscription.unsubscribe();
    }

    if (this.resultSubscription) {
      this.resultSubscription.unsubscribe();
    }
  }

  submit() {
    const checkedAssets = this.assets.filter(a => a['checked']);
    this.geofence.assetIds = [];
    for (const checkedAsset of checkedAssets) {
      this.geofence.assetIds.push(checkedAsset.id);
    }

    if (this.geofence.areaType === AreaType.Polygon && this.geofence.fencePolygon.length <= 2) {
      this.toasterService.pop('error', 'Geofence invalid', 'Please use the map to specify a valid geofence polygon');
      return;
    }

    if (this.geofence.areaType === AreaType.Circular && !this.geofence.fenceCenter) {
      this.toasterService.pop('error', 'Geofence invalid', 'Please use the map to specify a valid center for the geofence');
      return;
    }

    if (this.geofence.areaType === AreaType.Circular && this.geofence.radiusInMeters <= 0 || this.geofence.radiusInMeters >= 3185500) {
      this.toasterService.pop('error', 'Geofence invalid', 'Invalid geofence radius');
      return;
    }

    if (this.geofence.fenceType === undefined) {
      this.toasterService.pop('error', 'Fence Type invalid', 'Please specify the fence type');
      return;
    }

    this.geofence.emailsToNotify = this.joinedEmails.split(',').map(function (item) {
      return item.trim();
    });

    this.geofence.webhooksToNotify = this.joinedWebhooks.split(',').map(function (item) {
      return item.trim();
    });

    if (this.isEditable) {
      this.geofenceService.update(this.geofence)
        .subscribe(() => {
          this.router.navigate(['/geofences']);
        });
    } else {
      this.geofenceService.add(this.geofence)
        .subscribe(() => {
          this.router.navigate(['/geofences']);
        });
    }
  }

  clearPoints() {
    this.mapsService.endCurrentDraw();

    if (this.geofence.areaType === AreaType.Polygon) {
      this.geofence.fencePolygon = new Array<Point>();
      this.mapsService.startPolygonGeofenceDraw(this.geofence.fencePolygon);
    } else if (this.geofence.areaType === AreaType.Circular) {
      this.geofence.fenceCenter = new Point();
      this.geofence.radiusInMeters = 1000;
      this.mapsService.startCircularGeofenceDraw(this.geofence.fenceCenter, this.geofence.radiusInMeters);
    }
  }

  private updateAssetState() {
    if (this.geofence && this.geofence.assetIds && this.assets) {
      for (const asset of this.assets) {
        if (this.geofence.assetIds.indexOf(asset.id) !== -1) {
          asset['checked'] = true;
        }
      }
    }
  }

  onRadiusChanged(event) {
    this.mapsService.changeCircularGeofenceRadius(this.geofence.radiusInMeters);
  }

  onAreaTypeChanged(event) {
    this.clearPoints();
  }

  onAssetCheckChanged(event) {
    for (const asset of this.assets) {
      if (asset.id === event.source.name) {
        asset['checked'] = true;
      }
    }
  }
}
