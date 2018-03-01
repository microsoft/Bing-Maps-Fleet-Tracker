import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';

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

  getLocationName(location: Location) {
    return this.locationService.normalizeLocationName(location);
  }
}
