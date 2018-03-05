// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { DataService } from './data.service';
import { TrackingPoint } from '../shared/tracking-point';

@Injectable()
export class TripService {

  constructor(private dataService: DataService) { }

  getPoints(tripId: number, latitude: number, longitude: number) {
    return this.dataService.get<TrackingPoint>(`trip/${tripId}/points?lat=${latitude}&lon=${longitude}`);
  }
}
