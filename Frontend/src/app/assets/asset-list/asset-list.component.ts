// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { Subscription } from 'rxjs';
import { ToasterService } from 'angular2-toaster';

import { Asset } from '../asset';
import { AssetService } from '../asset.service';
import { MapsService } from '../../maps/maps.service';
import { LocationService } from '../../locations/location.service';
import { DateRange } from '../../shared/date-range';
import { Point } from '../../shared/point';
import { TrackingPoint } from '../../shared/tracking-point';
import { Trip } from '../../shared/trip';
import { Roles } from '../../shared/role';

import { takeWhile, skipWhile } from 'rxjs/operators';
import { AssetInfoDialogComponent } from '../asset-info-dialog/asset-info-dialog.component';


enum SelectedAssetState {
  ListSelected,
  PointsSelected,
  TripsSelected,
  NoneSelected
}

@Component({
  selector: 'app-asset-list',
  templateUrl: './asset-list.component.html',
  styleUrls: ['./asset-list.component.css']
})
export class AssetListComponent implements OnInit, OnDestroy {
  points: Point[];
  trips: Trip[];
  filter: string;
  Roles = Roles;

  assetsList: Asset[];
  selectedAsset: Asset;
  selectedAssetState: SelectedAssetState;
  showSnappedP = false;

  private selectedDateRange: DateRange;
  private lastCalledFunction: (asset: Asset, filterSelected: Boolean) => void;
  private subscription: Subscription;
  private isDrawing = false;
  private isAlive: boolean;

  constructor(
    private assetService: AssetService,
    private locationService: LocationService,
    private mapsService: MapsService,
    private toasterService: ToasterService,
    public dialog: MatDialog) {
    this.isAlive = true;
  }

  ngOnInit() {
    this.assetService.getAssets().pipe(
      takeWhile(() => this.isAlive),
      skipWhile(assets => assets.length === 0))
      .subscribe(assets => {
        this.assetsList = assets.sort((a, b) => {
          if (a.name < b.name) { return -1; }
          if (a.name > b.name) { return 1; }
          return 0;
        });
        this.showAllAssets();
      });
  }

  ngOnDestroy() {
    this.isAlive = false;
    this.unsubscribe();
  }

  showPoints(asset: Asset, filterSelected = false) {
    this.lastCalledFunction = this.showPoints;

    if (asset === this.selectedAsset && this.isAssetPointsSelected() && !filterSelected) {
      this.selectedAsset = null;
      this.selectedAssetState = SelectedAssetState.NoneSelected;
      this.showAllAssets();
    } else {
      this.selectedAsset = asset;
      this.selectedAssetState = SelectedAssetState.PointsSelected;
      this.toasterService.pop('info', '', 'Showing latest points of \" ' + this.selectedAsset.name + ' \"');
      this.unsubscribe();
      this.subscription = this.assetService.getPoints(asset.id, this.selectedDateRange, this.showSnappedP)
        .subscribe(points => {
          this.mapsService.showPoints(points, this.showSnappedP);
        });
    }
  }

  showAsset(asset: Asset) {
    this.lastCalledFunction = null;

    if (asset === this.selectedAsset && this.isAssetListSeleceted()) {
      this.selectedAsset = null;
    } else {
      this.selectedAsset = asset;
      this.selectedAssetState = SelectedAssetState.ListSelected;
      this.showAllAssets();
      this.toasterService.pop('info', '', 'Following asset ' + this.selectedAsset.name);
    }
  }

  private showAllAssets() {
    this.unsubscribe();
    this.subscription = this.assetService.getLatestPoints()
      .subscribe(points => {
        this.mapsService.showAssetsPositions(points);

        if (this.selectedAsset && !points[this.selectedAsset.name]) {
          this.toasterService.pop('error', '', 'Can\'t find position of ' + this.selectedAsset.name);
        } else if (this.selectedAsset) {
          this.mapsService.showAsset([this.selectedAsset, points[this.selectedAsset.name]]);
        }
      });
  }

  showTrips(asset: Asset, filterSelected = false) {
    this.lastCalledFunction = this.showTrips;

    if (asset === this.selectedAsset && this.isAssetTripsSelected() && !filterSelected) {
      this.selectedAssetState = SelectedAssetState.NoneSelected;
      this.selectedAsset = null;
      this.showAllAssets();
    } else {
      this.selectedAsset = asset;
      this.selectedAssetState = SelectedAssetState.TripsSelected;
      this.toasterService.pop('info', '', 'Showing latest trips of ' + this.selectedAsset.name);
      this.unsubscribe();
      this.subscription = this.assetService.getTrips(asset.id, this.selectedDateRange)
        .subscribe(trips => {
          this.trips = trips;
          this.mapsService.showTrips(trips);
        });
    }
  }

  selectTrip(trip): void {
    this.mapsService.showTrip(trip);
    this.isDrawing = true;
  }

  timeFilterChange(range) {
    if (this.lastCalledFunction) {
      this.selectedDateRange = range;
      this.lastCalledFunction(this.selectedAsset, true);
    }
  }

  showSnappedPoints() {
    this.showPoints(this.selectedAsset, true)
  }

  private unsubscribe() {
    if (this.subscription && !this.subscription.closed) {
      this.subscription.unsubscribe();
    }
    if (this.isDrawing) {
      this.mapsService.endCurrentDraw();
      this.isDrawing = false;
    }
  }

  openInfoDialog(): void {
    this.dialog.open(AssetInfoDialogComponent, {
      width: '600px',
    });
  }

  isAssetListSeleceted() {
    return this.selectedAssetState === SelectedAssetState.ListSelected;
  }

  isAssetTripsSelected() {
    return this.selectedAssetState === SelectedAssetState.TripsSelected;
  }

  isAssetPointsSelected() {
    return this.selectedAssetState === SelectedAssetState.PointsSelected;
  }
}

