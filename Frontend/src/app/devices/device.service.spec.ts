// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/* tslint:disable:no-unused-variable */

import { TestBed, inject } from '@angular/core/testing';
import { Observable } from 'rxjs';

import { DataService } from '../core/data.service';
import { Device } from './device';
import { DeviceService } from './device.service';
import { MockDataService } from '../../testing/mock-data.service';

import { of } from 'rxjs';

describe('DeviceService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        DeviceService,
        {
          provide: DataService,
          useClass: MockDataService
        }
      ]
    });
  });

  it('should call dataService.post("devices", device) in addDevice', inject([DeviceService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'post');
      const device = new Device();
      service.addDevice(device);
      expect(dataService.post).toHaveBeenCalledWith('devices', device);
    }));

  it('should call dataService.get("devices") in getDevices', inject([DeviceService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'get').and.returnValue(of({}));
      service.getDevices().subscribe(result => { });
      expect(dataService.get).toHaveBeenCalledWith('devices');
    }));

  it('should call dataService.get("devices/5/points") in getPoints', inject([DeviceService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'get').and.returnValue(of({}));
      service.getPoints('5').subscribe(result => { });
      expect(dataService.get).toHaveBeenCalledWith('devices/5/points');
    }));

  it('should call dataService.put("devices", "5", device, true) in updateDevice', inject([DeviceService, DataService],
    (service, dataService) => {
      spyOn(dataService, 'put');
      const device = { id: '5' };
      service.updateDevice(device);
      expect(dataService.put).toHaveBeenCalledWith('devices', '5', device, true);
    }));
});
