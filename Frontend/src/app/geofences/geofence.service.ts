// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';

import { DataService } from '../core/data.service';
import { Geofence } from '../shared/geofence';

@Injectable()
export class GeofenceService {
  constructor(private dataService: DataService) { }

  public getAll() {
    return this.dataService.get<Geofence>('geofences');
  }

  public get(id: number) {
    return this.dataService.getSingle<Geofence>('geofences', id);
  }

  public update(geofence: Geofence) {
    return this.dataService.put<Geofence>('geofences', geofence.id, geofence, true);
  }

  public add(geofence: Geofence) {
    return this.dataService.post<Geofence>('geofences', geofence);
  }

  public remove(geofence: Geofence) {
    return this.dataService.delete<Geofence>('geofences', geofence.id);
  }
}
