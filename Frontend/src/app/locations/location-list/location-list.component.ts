// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { Location } from '../../shared/location';
import { LocationService } from '../location.service';
import { MapsService } from '../../maps/maps.service';
import { Point } from '../../shared/point';
import { Roles } from '../../shared/role';
import { AssetService } from '../../assets/asset.service';
import { takeWhile } from 'rxjs/operators';
import { LocationsInfoDialogComponent } from '../locations-info-dialog/locations-info-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-location-list',
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.css']
})
export class LocationListComponent implements OnInit, OnDestroy {

  locations: Location[];
  selectedLocation: Location;
  filter: string;
  Roles = Roles;
  index = 1;
  singlePageSize = 10;
  showTable: boolean;
  assetsCount: { key: string, value: any }[];
  private isAlive: boolean;

  constructor(
    private locationService: LocationService,
    private assetService: AssetService,
    private mapsService: MapsService,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.isAlive = true;
    this.showFirstPageLocations();

    this.assetService.getLatestPoints().pipe(takeWhile(() => this.isAlive))
      .subscribe(points => {
        this.mapsService.showAssetsPositions(points, true);
      });
  }

  showFirstPageLocations() {
    this.locationService.getLocations().pipe(takeWhile(() => this.isAlive))
      .subscribe(locations => {
        this.locations = locations;
        this.showLocationsRange(this.locations.slice(0, this.singlePageSize - 1));
      });
  }

  ngOnDestroy() {
    this.isAlive = false;
  }

  private showLocationsRange(locationRange: Location[]) {
    this.mapsService.showLocationsPositions(locationRange);
  }

  showLocation(location: Location) {
    if (location === this.selectedLocation) {
      this.selectedLocation = null;
    } else {
      this.selectedLocation = location;
      this.mapsService.zoomToLocation(location);
    }
    this.getLocationInformation(location);
  }

  selectPage(index): void {
    this.index = index;
    const pagedLocations = this.locations.slice((index - 1) * this.singlePageSize, (index * this.singlePageSize) + 1);
    this.showLocationsRange(pagedLocations);
  }

  private getLocationInformation(location: Location) {
    this.assetsCount = null;
    this.locationService.getLocationAssetsCount(location).pipe(takeWhile(() => this.isAlive))
      .subscribe(assetsCount => {
        this.assetsCount = Object.keys(assetsCount).map(function (key) {
          return {
            key: key, value: assetsCount[key]
          };
        });

        this.showTable = (this.assetsCount.length >= 1);
      });
  }

  deleteLocation(location: Location) {
    this.locationService.deleteLocation(location);
  }

  openLocationDialog(): void {
    this.dialog.open(LocationsInfoDialogComponent, {
      width: '600px',
    });
  }
}
