// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { LocationDialogComponent } from './location-dialog/location-dialog.component';
import { LocationService } from '../locations/location.service';

import { Location } from '../shared/location';

@Injectable()
export class DialogService {

  constructor(private dialog: MatDialog,
  private locationService: LocationService) { }

  showLocationsDialog(): Observable<Location> {

    let dialogRef: MatDialogRef<LocationDialogComponent>;
    dialogRef = this.dialog.open(LocationDialogComponent);
    dialogRef.componentInstance.locationSubscription =  this.locationService.getLocations()
    .subscribe(locations => {
      dialogRef.componentInstance.locations = locations;
    });

    return dialogRef.afterClosed();
  }
}
