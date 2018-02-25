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
  undeterminedMessage: string;
  locationTypeString: string;
  private routerSubscription: Subscription;
  private resultSubscription: Subscription;
  private locationSubscription: Subscription;

  constructor(
    private locationService: LocationService,
    private mapService: MapsService,
    private route: ActivatedRoute,
    private router: Router,
    private toasterService: ToasterService) {
    this.location = new Location();
  }

  ngOnInit() {

    this.undeterminedMessage = 'Undetermined';
    this.mapService.startLocationPinDraw();
    this.routerSubscription = this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.isEditable = true;
        this.locationSubscription = this.locationService.getLocation(id).subscribe(location => {
          if (location) {
            this.location = location;
            this.setlocationString();
            this.setLocationTypeString();
            this.mapService.zoomToLocation(this.location);
          }
        });
      } else {
        this.locationString = this.undeterminedMessage;
      }
    });

    this.resultSubscription = this.mapService.getLocationPinResult()
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
    this.mapService.endLocationPinDraw();
    this.routerSubscription.unsubscribe();
  }

  changeAddress(): void {
    const address = this.location.address;
    this.mapService.geocodeQuery(address);
    this.mapService.getGeocodeResult().take(1).subscribe(point => {
      this.location.latitude = point.latitude;
      this.location.longitude = point.longitude;
      this.setlocationString();
      this.mapService.showLocationsPositions(new Map([[this.location.name, this.location]]));
    });

  }
  submit() {
    if (this.locationString === this.undeterminedMessage) {
      this.toasterService.pop('error', 'Invalid Input', 'Please add the location pin on the map');
      return;
    }

    if (this.isEditable) {
      if (this.didLocationChange) {
        this.location.interestLevel = InterestLevel.Manual;
      }
      this.locationService.updateLocation(this.location)
        .subscribe(() => this.router.navigate(['/locations']));
    } else {
      this.location.interestLevel = InterestLevel.Manual;
      this.locationService.addLocation(this.location)
        .subscribe(() => this.router.navigate(['/locations']));
    }
  }

  private setlocationString() {
    this.locationString = '(' + this.location.latitude + ' , ' + this.location.longitude + ' )';
  }

  private setLocationTypeString() {
    if (this.location.interestLevel === InterestLevel.Manual) {
      this.locationTypeString = 'Created/Edited Manually';
    } else {
      this.locationTypeString = 'Automatically Generated';
    }
  }
}
