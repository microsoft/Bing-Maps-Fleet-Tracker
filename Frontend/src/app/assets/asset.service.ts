import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/map';

import { Asset } from './asset';
import { DataService } from '../core/data.service';
import { DateRange } from '../shared/date-range';
import { TrackingPoint } from '../shared/tracking-point';
import { Trip } from '../shared/trip';

@Injectable()
export class AssetService {

  constructor(private dataService: DataService) {
  }

  addAsset(asset: Asset): Observable<void> {
    return this.dataService.post<Asset>('assets', asset);
  }

  getAssets(): Observable<Asset[]> {
    return this.dataService.get<Asset>('assets');
  }

  getAsset(id: string): Observable<Asset> {
    return this.dataService.getSingle('assets', id);
  }

  getPoints(id: string, dateRange?: DateRange): Observable<TrackingPoint[]> {
    return this.dataService.get<TrackingPoint>(`assets/${id}/points`)
      .map(points => {
        if (!dateRange) {
          return points;
        }

        return points.filter(p => p.time >= +dateRange.from && p.time <= +dateRange.to);
      });
  }

  getLatestPoints(): Observable<{ [key: string]: TrackingPoint }> {
    return Observable
      .interval(3 * 1000)
      .startWith(0)
      .flatMap(() => {
        return this.dataService.getSingleNoCache<{ [key: string]: TrackingPoint }>(`assets/all/positions`);
      });
  }

  getTrips(id: string, dateRange?: DateRange): Observable<Trip[]> {
    return this.dataService.get<Trip>(`assets/${id}/trips`)
      .map(trips => {
        if (!dateRange) {
          return trips;
        }

        return trips.filter(t => {
          return t.startTimeStampUtc >= +dateRange.from && t.startTimeStampUtc <= +dateRange.to;
        });
      });
  }
}
