import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { Geofence, FenceType } from '../../shared/geofence';
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
  FenceType = FenceType;
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

    this.routeSubscription = this.route.params.subscribe(params => {
      const id = params['id'];

      if (id) {
        this.geofenceSubscription = this.geofenceService.get(id).subscribe(geofence => {
          if (geofence != null) {
            this.geofence = geofence;
            this.joinedEmails = this.geofence.emailsToNotify.join(', ');
            this.mapsService.startGeofenceDraw(geofence.fencePolygon);
            this.updateAssetState();
            this.geofence.fencePolygon = [];
          }
        });

        this.isEditable = true;
      } else {
        this.mapsService.startGeofenceDraw();
      }
    });

    this.assetsSubscription = this.assetService.getAssets().subscribe(assets => {
      this.assets.splice(0, this.assets.length);
      assets.forEach(a => {
        this.assets.push(a);
      });

      this.updateAssetState();
    });

    this.resultSubscription = this.mapsService.getGeodenceDrawResult()
      .subscribe(result => {
        this.geofence.fencePolygon = result;
      });

    this.toasterService.pop('info', 'Draw Geofence', 'Please use the map to specify your geofence');
  }

  ngOnDestroy() {
    this.mapsService.endGeofenceDraw();

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

    if (this.geofence.fencePolygon.length <= 2) {
      this.toasterService.pop('error', 'Geofence invalid', 'Please use the map to specify a valid geofence');
      return;
    }

    if (this.geofence.fenceType === undefined) {
      this.toasterService.pop('error', 'Fence Type invalid', 'Please specify the fence type');
      return;
    }

    this.geofence.emailsToNotify = this.joinedEmails.split(',').map(function (item) {
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
    this.geofence.fencePolygon = new Array<Point>();
    this.mapsService.resetGeofenceDraw(this.geofence.fencePolygon);
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

  onCheckChanged(event) {
    for (const asset of this.assets) {
      if (asset.id === event.source.name) {
        asset['checked'] = true;
      }
    }
  }
}
