// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { map } from 'rxjs/operators';
import { Asset } from './asset';
import { DataService } from '../core/data.service';
import { DateRange } from '../shared/date-range';
import { TrackingPoint } from '../shared/tracking-point';
import { Trip } from '../shared/trip';

import { interval } from 'rxjs';
import { startWith, flatMap } from 'rxjs/operators';

@Injectable()
export class AssetService {

  constructor(private dataService: DataService) {
  }

  addAsset(asset: Asset): Observable<void> {
    return this.dataService.post<Asset>('assets', asset);
  }

  updateAsset(asset: Asset): Observable<void> {
    return this.dataService.put<Asset>('assets', asset.id, asset, true);
  }

  deleteAsset(asset: Asset): Observable<void> {
    return this.dataService.delete<Asset>('assets', asset.id);
  }

  getAssets(): Observable<Asset[]> {
    return this.dataService.get<Asset>('assets');
  }

  getAsset(id: string): Observable<Asset> {
    return this.dataService.getSingle('assets', id);
  }

  getPoints(id: string, dateRange?: DateRange): Observable<TrackingPoint[]> {
    return this.dataService.get<TrackingPoint>(`assets/${id}/points`).pipe(
      map(points => {
        if (!dateRange) {
          return points;
        }

        return points.filter(p => p.time >= +dateRange.from && p.time <= +dateRange.to);
      }));
  }

  getLatestPoints(): Observable<{ [key: string]: TrackingPoint }> {
    return interval(3 * 1000).pipe(
      startWith(0),
      flatMap(() => {
        return this.dataService.getSingleNoCache<{ [key: string]: TrackingPoint }>(`assets/all/positions`);
      })
    )
  }

  getTrips(id: string, dateRange?: DateRange): Observable<Trip[]> {
    return this.dataService.get<Trip>(`assets/${id}/trips`).pipe(
      map(trips => {
        if (!dateRange) {
          return trips;
        }

        return trips.filter(t => {
          return Date.parse(t.startTimeUtc) >= +dateRange.from && Date.parse(t.startTimeUtc) <= +dateRange.to;
        });
      }));
  }
}
