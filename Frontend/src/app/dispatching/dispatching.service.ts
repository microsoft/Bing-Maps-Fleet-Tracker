// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

import { DispatchingParameters } from './dispatching-parameters';
import { DispatchingResults } from './dispatching-results';
import { DataService } from '../core/data.service';
import { Location } from '../shared/location';
import { MapsService } from '../maps/maps.service';

@Injectable()
export class DispatchingService {

  private dispatchingResults: Observable<DispatchingResults[]>;
  private dispatchingParameters: DispatchingParameters;
  private pinsAdded: Location[];

  constructor(
    private dataService: DataService,
    private mapService: MapsService,
    private router: Router) {}

  callDisaptchingAPI(dispatchingParameters: DispatchingParameters) {
    this.dispatchingParameters = dispatchingParameters;
    this.dispatchingResults = this.dataService.post<DispatchingParameters>('dispatching', dispatchingParameters);
    this.router.navigate(['/dispatching/show']);
  }

  getDispatchingResults(): Observable<DispatchingResults[]> {
    return this.dispatchingResults;
  }

  getDispatchingPinsResult(): Observable<any> {
    return this.mapService.getDispatchingPinsResult()
  }
  
  getPinsAdded(): Location[] {
    return this.pinsAdded;
  }

  getDispatchingParameters(): DispatchingParameters{
    return this.dispatchingParameters;
  }

  savePinsAdded(pins: Location[]) {
    this.pinsAdded = pins;
  }
}
