// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { Subscription } from 'rxjs';

import { LocationService } from '../../locations/location.service';
import { Location } from '../../shared/location';

@Component({
  selector: 'app-location-dialog',
  templateUrl: './location-dialog.component.html',
  styleUrls: ['./location-dialog.component.css']
})
export class LocationDialogComponent implements OnInit, OnDestroy {

  selectedLocation: Location;
  filter: string;
  public locationSubscription: Subscription;
  public locations: Location[];

  constructor(
    private dialogRef: MatDialogRef<LocationDialogComponent>,
    private locationService: LocationService) { }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.locationSubscription && !this.locationSubscription.closed) {
      this.locationSubscription.unsubscribe();
    }
  }

  onSelected(location: Location) {
    this.selectedLocation = location;
  }

  onButtonClicked() {
    this.dialogRef.close(this.selectedLocation);
  }
}
