// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

import { Location, InterestLevel } from '../../shared/location';
import { Point } from '../../shared/point';
import { LocationService } from '../location.service';
import { ToasterService } from 'angular2-toaster';
import { MapsService } from '../../maps/maps.service';

@Component({
  selector: 'app-location-editor',
  templateUrl: './location-editor.component.html',
  styleUrls: ['./location-editor.component.css']
})
export class LocationEditorComponent implements OnInit, OnDestroy {
  location: Location;
  didLocationChange: Boolean;
  isEditable: Boolean;
  locationString: string;
  locationTypeString: string;
  private isAlive: boolean;
  private readonly undeterminedMessage = 'Undetermined';

  constructor(
    private locationService: LocationService,
    private mapService: MapsService,
    private route: ActivatedRoute,
    private router: Router,
    private toasterService: ToasterService) {
    this.location = new Location();
  }

  ngOnInit() {
    this.isAlive = true;
    this.mapService.startLocationPinDraw();
    this.route.params
      .takeWhile(() => this.isAlive)
      .subscribe(params => {
        const id = params['id'];
        if (id) {
          this.isEditable = true;
          this.locationService.getLocation(id)
            .takeWhile(() => this.isAlive)
            .subscribe(location => {
              if (location) {
                this.location = location;
                this.setlocationString();
                this.mapService.showLocationsPositions([this.location]);
              }
            });
        } else {
          this.locationString = this.undeterminedMessage;
        }
      });

    this.mapService.getLocationPinResult()
      .takeWhile(() => this.isAlive)
      .subscribe(location => {
        if (this.location.latitude !== location.latitude || this.location.longitude !== location.longitude) {
          this.didLocationChange = true;
          this.location.latitude = location.latitude;
          this.location.longitude = location.longitude;
          this.location.address = location.address;
          if (!this.location.name) {
            this.location.name = location.name;
          }
          this.setlocationString();
        }
      });
  }

  ngOnDestroy() {
    this.mapService.endCurrentDraw();
    this.isAlive = false;
  }

  changeAddress(): void {
    const address = this.location.address;
    this.mapService.geocodeQuery(address);
    this.mapService.getGeocodeResult().take(1).subscribe(point => {
      this.location.latitude = point.latitude;
      this.location.longitude = point.longitude;
      this.setlocationString();
      this.mapService.showLocationsPositions([this.location]);
    });
  }

  submit() {
    if (this.locationString === this.undeterminedMessage) {
      this.toasterService.pop('error', 'Invalid Input', 'Please add the location pin on the map');
      return;
    }

    if (this.isEditable) {
      this.locationService.updateLocation(this.location)
        .subscribe(() => this.router.navigate(['/locations']));
    } else {
      this.locationService.addLocation(this.location)
        .subscribe(() => this.router.navigate(['/locations']));
    }
  }

  private setlocationString() {
    this.locationString = '(' + this.location.latitude + ' , ' + this.location.longitude + ' )';
  }
}
