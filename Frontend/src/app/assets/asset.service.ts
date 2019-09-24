// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable, combineLatest } from 'rxjs';

import { map } from 'rxjs/operators';
import { Asset } from './asset';
import { DataService } from '../core/data.service';
import { DateRange } from '../shared/date-range';
import { TrackingPoint } from '../shared/tracking-point';
import { Trip } from '../shared/trip';
import { BingMapsService } from 'app/maps/bing-maps.service';

import { interval } from 'rxjs';
import { startWith, flatMap, mergeMap, take, switchMap, skip } from 'rxjs/operators';

@Injectable()
export class AssetService {

  private snapToRoadUrl = "https://dev.virtualearth.net/REST/v1/Routes/SnapToRoad?"

  constructor(
    private dataService: DataService,
    private bingMapsService: BingMapsService) {
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

  getPoints(id: string, dateRange?: DateRange, snapPoints = false): Observable<TrackingPoint[]> {
    var obsPoints = this.dataService.get<TrackingPoint>(`assets/${id}/points`).pipe(
      map(points => {
        if (!dateRange) {
          return points;
        }
        return points.filter(p => p.time >= +dateRange.from && p.time <= +dateRange.to);
      }));

    if (!snapPoints) {
      return obsPoints
    }

    var obsBingKey = this.dataService.get<JSON>(`settings/subscriptionkeys`).pipe(
      map(keys => {

        let bingMapsKey = '';
        for (var key of keys) {
          if (key['id'] === 'BingMaps') {
            bingMapsKey = key['keyValue'];
            break;
          }
        }
        return bingMapsKey;
      }),
      skip(1)
    )

    return combineLatest([obsPoints, obsBingKey]).pipe(
      map((values) => {
        return { "points": values[0], "key": values[1] }
      }),
      map((value) => {
        return value;
      }),
      switchMap(data => {
        let prevP = null
        let points = data["points"].filter(p => {
          if (prevP) {
            return this.bingMapsService.computeDistanceBetween(prevP, p) > 2.5;
          }
          prevP = p;
          return true;
        })
        var pointsChunks = this.chunks<TrackingPoint>(points, 50)
        var obsChunks = [];
        for (var i in pointsChunks) {
          obsChunks.push(this.snapChunk(i, pointsChunks[i], data["key"]))
        }
        return combineLatest(obsChunks);
      }),
      map(l => {
        return [].concat.apply([], l)
      })
    )
  }


  snapChunk(index, chunk, key) {
    return this.dataService.getNoCache<JSON>(`${this.snapToRoadUrl}${this.generateUrl(chunk)}&key=${key}`, false, false).pipe(
      map((response) => {
        var snappedPoints = response["resourceSets"][0]["resources"][0]["snappedPoints"]
        var result = this.snapPointsToRoad(chunk, snappedPoints);
        return result;
      })
    );
  }

  snapPointsToRoad(trackingPoints: Array<TrackingPoint>, snappedPoints) {
    var result = []
    for (var i in snappedPoints) {
      let p = snappedPoints[i]
      let index = p.index;
      trackingPoints[index].longitude = p.coordinate.longitude;
      trackingPoints[index].latitude = p.coordinate.latitude;
      result.push(trackingPoints[index]);
    }
    return result;
  }

  generateUrl(points: Array<TrackingPoint>) {
    var pointsString = "points=";
    for (var i in points) {
      let p = points[i];
      pointsString += p.latitude;
      pointsString += ",";
      pointsString += p.longitude;
      pointsString += ";";
    }
    return pointsString.slice(0, -1);
  }

  chunks<T>(L: Array<T>, size: number): Array<Array<T>> {
    var list = [];
    while (L.length > 0) {
      list.push(L.splice(0, size));
    }
    return list
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
