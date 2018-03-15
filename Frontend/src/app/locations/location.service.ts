// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { DataService } from '../core/data.service';
import { Location } from '../shared/location';
import { Point } from '../shared/point';

@Injectable()
export class LocationService {
  constructor(private dataSevrice: DataService) { }

  addLocation(location: Location): Observable<void> {
    return this.dataSevrice.post<Location>('locations', location);
  }

  getLocations(): Observable<Location[]> {
    return this.dataSevrice.get<Location>('locations');
  }

  getLocation(id: number): Observable<Location> {
    return this.dataSevrice.getSingle<Location>('locations', id);
  }

  updateLocation(location: Location): Observable<void> {
    return this.dataSevrice.put<Location>('locations', location.id, location, true);
  }

  getLocationAssetsCount(location: Location): Observable<Map<string, number>> {
    return this.dataSevrice.getSingleNoCache<Map<string, number>>(`locations/${location.id}/assetsCount`);
  }
}
