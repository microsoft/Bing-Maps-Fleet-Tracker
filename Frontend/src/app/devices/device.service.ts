// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { DateRange } from '../shared/date-range';
import { DataService } from '../core/data.service';
import { Device } from './device';
import { EnvironmentSettings, EnvironmentSettingsService } from '../core/environment-settings.service';
import { TrackingPoint } from './../shared/tracking-point';

@Injectable()
export class DeviceService {

  constructor(private dataService: DataService,
    private envSettingsService: EnvironmentSettingsService) { }

  addDevice(device: Device): Observable<void> {
    const update = new Map().set('update', 'false');
    return this.dataService.post<Device>('devices', device, update);
  }

  getDevices(): Observable<Device[]> {
    return this.dataService.get<Device>('devices');
  }

  getDevice(id: string): Observable<Device> {
    return this.dataService.getSingle<Device>('devices', id);
  }

  getToken(id: string): Observable<string> {
    return this.dataService.post<string>(`devices/${id}/token`, '');
  }

  getPoints(id: string, dateRange?: DateRange): Observable<TrackingPoint[]> {
    return this.dataService.get<TrackingPoint>(`devices/${id}/points`)
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
        return this.dataService.getSingleNoCache<{ [key: string]: TrackingPoint }>(`devices/all/positions`);
      });
  }

  updateDevice(device: Device): Observable<void> {
    return this.dataService.put<Device>('devices', device.id, device, true);
  }

  deleteDevice(device: Device): Observable<void> {
    return this.dataService.delete<Device>('devices', device.id);
  }

  getProvisioningQrCodeUrl(nonce: String): string {
    return `${this.envSettingsService.getEnvironmentVariable(EnvironmentSettings.backendUrl)}/devices/qrcode?nonce=${nonce}`;
  }

  getDeviceAdditionNotificationUrl(): string {
    return `${this.envSettingsService.getEnvironmentVariable(EnvironmentSettings.backendUrl).replace('api', '')}deviceAddition`;
  }
}
