// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { Geofence, FenceType } from '../../shared/geofence';
import { Asset } from '../../assets/asset';
import { TrackingPoint } from '../../shared/tracking-point';
import { AssetService } from '../../assets/asset.service';
import { GeofenceService } from '../geofence.service';
import { MapsService } from '../../maps/maps.service';

@Component({
  selector: 'app-geofence-list',
  templateUrl: './geofence-list.component.html',
  styleUrls: ['./geofence-list.component.css']
})
export class GeofenceListComponent implements OnInit, OnDestroy {
  geofences: Observable<Geofence[]>;
  retrievedGeofences: Geofence[];
  selectedGeofence: Geofence;
  FenceType = FenceType;
  filter: string;
  private isAlive: boolean;

  constructor(
    private geofenceService: GeofenceService,
    private assetService: AssetService,
    private mapsService: MapsService) { }

  ngOnInit() {
    this.isAlive = true;
    this.geofences = this.geofenceService.getAll();
    this.geofences
      .takeWhile(() => this.isAlive)
      .subscribe(geofences => {
        this.mapsService.showGeofences(geofences);
        this.retrievedGeofences = geofences;
      });

    this.assetService.getLatestPoints()
      .takeWhile(() => this.isAlive)
      .subscribe(points => {
        this.mapsService.showAssetsPositions(points, true);
      });
  }

  ngOnDestroy(): void {
    this.isAlive = false;
  }

  showGeofence(geofence: Geofence) {
    if (this.selectedGeofence === geofence) {
      this.mapsService.showGeofences(this.retrievedGeofences);
      this.selectedGeofence = null;
    } else {
      this.mapsService.showGeofence(geofence);
      this.selectedGeofence = geofence;
    }
  }

  deleteGeofence(geofence: Geofence) {
    this.geofenceService.remove(geofence);
  }
}
