import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { Geofence, FenceType } from '../../shared/geofence';
import { GeofenceService } from '../geofence.service';
import { MapsService } from '../../maps/maps.service';

@Component({
  selector: 'app-geofence-list',
  templateUrl: './geofence-list.component.html',
  styleUrls: ['./geofence-list.component.css']
})
export class GeofenceListComponent implements OnInit, OnDestroy {
  retrievedGeofences: Geofence[];
  selectedGeofence: Geofence;
  filter: string;
  geofences: Observable<Geofence[]>;
  geofencesSubscription: Subscription;
  FenceType = FenceType;

  constructor(private geofenceService: GeofenceService, private mapsService: MapsService) { }

  ngOnInit() {
    this.geofences = this.geofenceService.getAll();
    this.geofencesSubscription = this.geofences.subscribe(geofences => {
      this.mapsService.showGeofences(geofences);
      this.retrievedGeofences = geofences;
    });
  }

  ngOnDestroy(): void {
    if (this.geofencesSubscription && !this.geofencesSubscription.closed) {
      this.geofencesSubscription.unsubscribe();
    }
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
}
