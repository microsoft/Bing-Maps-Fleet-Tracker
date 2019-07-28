// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/* tslint:disable:no-unused-variable */

import { TestBed, inject } from '@angular/core/testing';
import { Observable } from 'rxjs';
import { Asset } from './asset';
import { AssetService } from './asset.service';
import { DataService } from '../core/data.service';
import { DeviceService } from '../devices/device.service';
import { MockDataService } from '../../testing/mock-data.service';

import 'rxjs/add/observable/of';

describe('AssetService', () => {

  beforeEach(() => {

    TestBed.configureTestingModule({
      providers: [
        AssetService,
        DeviceService,
        {
          provide: DataService,
          useClass: MockDataService
        }
      ]
    });
  });

  it('should call dataService.post("assets", asset) in addAsset', inject([AssetService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'post');
      const asset = { id: '123' };
      service.addAsset(asset);
      expect(dataService.post).toHaveBeenCalledWith('assets', asset);
    }));

  it('should call dataService.get("assets") in getAssets', inject([AssetService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'get').and.returnValue(Observable.of({}));
      service.getAssets().subscribe(result => { });
      expect(dataService.get).toHaveBeenCalledWith('assets');
    }));

  it('should call dataService.get("assets/5/points") in getPoints', inject([AssetService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'get').and.returnValue(Observable.of({}));
      service.getPoints('5').subscribe(result => { });
      expect(dataService.get).toHaveBeenCalledWith('assets/5/points');
    }));

  it('should call dataService.get("assets/5/trips") in getTrips', inject([AssetService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'get').and.returnValue(Observable.of({}));
      service.getTrips('5').subscribe(result => { });
      expect(dataService.get).toHaveBeenCalledWith('assets/5/trips');
    }));
});
