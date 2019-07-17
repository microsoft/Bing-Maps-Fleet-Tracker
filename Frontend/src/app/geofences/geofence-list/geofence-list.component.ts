// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { Geofence, FenceType } from '../../shared/geofence';
import { Asset } from '../../assets/asset';
import { TrackingPoint } from '../../shared/tracking-point';
import { AssetService } from '../../assets/asset.service';
import { GeofenceService } from '../geofence.service';
import { MapsService } from '../../maps/maps.service';
import { takeWhile } from 'rxjs/operators';
import { GeofencesInfoDialogComponent } from '../geofences-info-dialog/geofences-info-dialog.component';
import { MatDialog } from '@angular/material/dialog';

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
    private mapsService: MapsService,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.isAlive = true;
    this.geofences = this.geofenceService.getAll();
    this.geofences.pipe(takeWhile(() => this.isAlive))
      .subscribe(geofences => {
        this.mapsService.showGeofences(geofences);
        this.retrievedGeofences = geofences;
      });

    this.assetService.getLatestPoints().pipe(takeWhile(() => this.isAlive))
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

  openGeoDialog():void {
    this.dialog.open(GeofencesInfoDialogComponent, {  
      width: '600px',
    });
  }
}
