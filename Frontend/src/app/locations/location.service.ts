// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { DataService } from '../core/data.service';
import { Location } from '../shared/location';
import { Point } from '../shared/point';

@Injectable()
export class LocationService {
  constructor(private dataService: DataService) { }

  addLocation(location: Location): Observable<void> {
    return this.dataService.post<Location>('locations', location);
  }

  getLocations(): Observable<Location[]> {
    return this.dataService.get<Location>('locations');
  }

  getLocation(id: number): Observable<Location> {
    return this.dataService.getSingle<Location>('locations', id);
  }

  updateLocation(location: Location): Observable<void> {
    return this.dataService.put<Location>('locations', location.id, location, true);
  }

  getLocationAssetsCount(location: Location): Observable<Map<string, number>> {
    return this.dataService.getSingleNoCache<Map<string, number>>(`locations/${location.id}/assetsCount`);
  }

  deleteLocation(location: Location): Observable<void> {
    return this.dataService.delete<Location>('locations', location.id);
  }
}
