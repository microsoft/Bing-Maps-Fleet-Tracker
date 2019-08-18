// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';

import { AssetService } from '../../assets/asset.service';
import { DialogService } from '../dialog.service';
import { DispatchingService } from '../dispatching.service';
import { LocationService } from '../../locations/location.service';
import { MapsService } from '../../maps/maps.service';
import { ToasterService } from 'angular2-toaster';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Location } from '../../shared/location';
import { Asset, AssetType } from '../../assets/asset';

import { takeWhile } from 'rxjs/operators';

import {
  DistanceUnit,
  DimensionUnit,
  WeightUnit,
  DispatchingParameters,
  OptimizeValue,
  RouteAttributes,
  TimeType
} from '../dispatching-parameters';

import {
  AvoidOptions,
  MinimizeOptions,
  OptimizeOptions,
  HazardousMaterialOptions,
  HazardousPermitOptions
} from '../dispatching-editor-options';
import { DispatchingInfoDialogComponent } from '../dispatching-info-dialog/dispatching-info-dialog.component';

@Component({
  selector: 'app-dispatching-editor',
  templateUrl: './dispatching-editor.component.html',
  styleUrls: ['./dispatching-editor.component.css']
})
export class DispatchingEditorComponent implements OnInit, OnDestroy {

  dispatchingParameters: DispatchingParameters;
  assets: Asset[];
  selectedAsset: Asset;
  showDirections: boolean;
  showTruckOptions: boolean;
  pinsAdded: Location[];
  departureDate: Date;

  routeColors = ['Blue', 'Red', 'Green', 'Cyan'];
  colorSelected: number;

  AssetType = AssetType;
  DistanceUnit = DistanceUnit;
  WeightUnit = WeightUnit;
  DimensionUnit = DimensionUnit;
  AvoidOptions = AvoidOptions;
  MinimizeOptions = MinimizeOptions;
  OptimizeOptions = OptimizeOptions;
  HazardMaterialOptions = HazardousMaterialOptions;
  HazardousPermitOptions = HazardousPermitOptions;

  showAvoidList: boolean;
  showMinimizeList: boolean;
  showOptimizeList: boolean;
  showLoadedAssetPropList: boolean;
  showTimingList: boolean;
  showHazardMaterialList: boolean;
  showHazardPermitList: boolean;
  showResultsList: boolean;
  showPinsList: boolean;

  private isAlive: boolean;

  constructor(
    private assetService: AssetService,
    private dialogService: DialogService,
    private dispatchingService: DispatchingService,
    private locationService: LocationService,
    private mapsService: MapsService,
    private toasterService: ToasterService,
    public dialog: MatDialog) {
    this.resetAllData();
  }

  ngOnInit() {
    this.isAlive = true;
    this.mapsService.getDispatchingPinsResult().pipe(
      takeWhile(() => this.isAlive))
      .subscribe(result => {
        if (result.length) {
          this.pinsAdded = result;
        }
      });

    this.assetService.getAssets().pipe(
      takeWhile(() => this.isAlive))
      .subscribe(assets => {
        if (!this.assets && assets.length) {
          this.assets = assets;
        }
      });

    this.assetService.getLatestPoints().pipe(
      takeWhile(() => this.isAlive))
      .subscribe(points => {
        this.mapsService.showAssetsPositions(points, true);
      });
  }

  ngOnDestroy() {
    this.mapsService.endCurrentDraw();
    this.isAlive = false;
  }

  openDialog() {
    this.dialogService.showLocationsDialog()
      .subscribe(location => {
        if (location) {
          location.name = location.name + `(${this.pinsAdded.length + 1})`;
          this.pinsAdded.push(location);
          this.mapsService.resetDispatchingDraw(this.pinsAdded);
        }
      });
  }

  checkSelected(asset: Asset) {
    this.showTruckOptions = asset.assetType === AssetType.Truck;
  }

  clearPoints() {
    this.pinsAdded = new Array<Location>();
    this.mapsService.resetDispatchingDraw();
  }

  showRoute() {
    if (this.pinsAdded.length < 2 || this.pinsAdded.length > 20) {
      this.toasterService.pop('error', 'Invalid Input', 'Number of Points per route has to be between 2 and 20');
      return;
    }

    if (!this.selectedAsset) {
      this.toasterService.pop('error', 'Invalid Input', 'Please select an asset');
      return;
    }

    this.setAllDispatchingParameters();

    this.dispatchingService.callDisaptchingAPI(this.dispatchingParameters);
  }

  onCheckChanged(event, Options) {
    Options.forEach(option => {
      if (option.name === event.source.name) {
        option.disabled = !option.disabled;
      }
    });
  }

  resetAllData() {
    this.dispatchingParameters = new DispatchingParameters();

    this.dispatchingParameters.routeAttributes = [];
    this.dispatchingParameters.optimize = OptimizeValue.Time;
    this.dispatchingParameters.distanceUnit = DistanceUnit.Kilometer;
    this.dispatchingParameters.weightUnit = WeightUnit.Kilogram;
    this.dispatchingParameters.dimensionUnit = DimensionUnit.Meter;
    this.dispatchingParameters.timeType = TimeType.Departure;
    this.dispatchingParameters.getAlternativeCarRoute = false;

    this.selectedAsset = null;
    this.colorSelected = 0;
    this.showDirections = false;
    this.pinsAdded = [];

    this.AvoidOptions.forEach(option => option.isChecked = false);
    this.MinimizeOptions.forEach(option => option.isChecked = false);
    this.HazardMaterialOptions.forEach(option => option.isChecked = false);
    this.HazardousPermitOptions.forEach(option => option.isChecked = false);

    this.clearPoints();
  }

  private setAllDispatchingParameters() {
    this.dispatchingParameters.avoid = this.AvoidOptions
      .filter(option => option.isChecked).map(option => option.value);

    this.dispatchingParameters.hazardousMaterials = this.HazardMaterialOptions
      .filter(option => option.isChecked).map(option => option.value);

    this.dispatchingParameters.hazardousPermits = this.HazardousPermitOptions
      .filter(option => option.isChecked).map(option => option.value);

    this.dispatchingParameters.assetId = this.selectedAsset.id;

    if (this.showDirections) {
      this.dispatchingParameters.routeAttributes.push(RouteAttributes.ExcludeItinerary);
    }

    this.dispatchingParameters.wayPoints = this.pinsAdded;
    this.dispatchingService.savePinsAdded(this.pinsAdded);
    this.mapsService.setRouteColor(this.colorSelected);
  }

  delete_pin(name: string) {
    this.pinsAdded = this.pinsAdded.filter(item => item.name !== name);
    this.mapsService.resetDispatchingDraw(this.pinsAdded);
  }

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.pinsAdded, event.previousIndex, event.currentIndex);
  }

  togglePinsList() {
    this.showPinsList = this.showPinsList ? false : true;
  }

  openDispatchingInfoDialog(): void {
    this.dialog.open(DispatchingInfoDialogComponent, {
      width: '600px',
    });
  }
}
