import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { DispatchingParameters } from './dispatching-parameters';
import { DispatchingResults } from './dispatching-results';
import { DataService } from '../core/data.service';
import { Location } from '../shared/location';

@Injectable()
export class DispatchingService {

  private dispatchingResults: Observable<DispatchingResults[]>;
  private pinsAdded: Location[];

  constructor(
    private dataService: DataService,
    private router: Router) {}

  callDisaptchingAPI(dispatchingParameters: DispatchingParameters) {
    this.dispatchingResults = this.dataService.post<DispatchingParameters>('dispatching', dispatchingParameters);
    this.router.navigate(['/dispatching/show']);
  }

  getDispatchingResults(): Observable<DispatchingResults[]> {
    return this.dispatchingResults;
  }

  getPinsAdded(): Location[] {
    return this.pinsAdded;
  }

  savePinsAdded(pins: Location[]) {
    this.pinsAdded = pins;
  }
}
