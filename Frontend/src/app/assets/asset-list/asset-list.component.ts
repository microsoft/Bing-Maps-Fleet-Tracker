// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
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

  assets: Observable<Asset[]>;
  points: Point[];
  trips: Trip[];
  filter: string;
  Roles = Roles;

  selectedAsset: Asset;
  selectedAssetState: SelectedAssetState;
  private selectedDateRange: DateRange;
  private lastCalledFunction: (asset: Asset, filterSelected: Boolean) => void;
  private subscription: Subscription;
  private isAlive: boolean;
  private assetsList: Asset[];

  constructor(
    private assetService: AssetService,
    private locationService: LocationService,
    private mapsService: MapsService,
    private toasterService: ToasterService) {
    this.isAlive = true;
  }

  ngOnInit() {
    this.assets = this.assetService.getAssets();
    this.assets
      .takeWhile(() => this.isAlive)
      .skipWhile(assets => assets.length === 0)
      .take(1)
      .subscribe(assets => {
        this.assetsList = assets;
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
      this.toasterService.pop('info', '', 'Showing latest points of \" ' + this.selectedAsset.id + ' \"');
      this.unsubscribe();
      this.subscription = this.assetService.getPoints(asset.id, this.selectedDateRange)
        .subscribe(points => {
          this.mapsService.showPoints(points);
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
      this.toasterService.pop('info', '', 'Following asset ' + this.selectedAsset.id);
    }
  }

  private showAllAssets() {
    this.unsubscribe();
    this.subscription = this.assetService.getLatestPoints()
      .subscribe(points => {
        const mappedAssets = new Array<[Asset, TrackingPoint]>();

        for (const key of Object.keys(points)) {
          const value = points[key];
          const asset = this.assetsList.find(val => val.id === key);
          mappedAssets.push([asset, value]);
        }

        this.mapsService.showAssetsPositions(mappedAssets);

        if (this.selectedAsset && !points[this.selectedAsset.id]) {
          this.toasterService.pop('error', '', 'Can\'t find position of ' + this.selectedAsset.id);
        } else if (this.selectedAsset) {
          this.mapsService.showAsset([this.selectedAsset, points[this.selectedAsset.id]]);
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
      this.toasterService.pop('info', '', 'Showing latest trips of ' + this.selectedAsset.id);
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
  }

  timeFilterChange(range) {
    if (this.lastCalledFunction) {
      this.selectedDateRange = range;
      this.lastCalledFunction(this.selectedAsset, true);
    }
  }

  private unsubscribe() {
    if (this.subscription && !this.subscription.closed) {
      this.subscription.unsubscribe();
    }
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

