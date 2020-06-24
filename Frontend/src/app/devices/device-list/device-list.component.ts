// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ToasterService } from 'angular2-toaster';

import { DateRange } from '../../shared/date-range';
import { TrackingPoint } from '../../shared/tracking-point';
import { Device } from '../device';
import { DeviceService } from '../device.service';
import { MapsService } from '../../maps/maps.service';
import { Roles } from '../../shared/role';
import { MatDialog } from '@angular/material/dialog';
import { DevicesInfoDialogComponent } from '../devices-info-dialog/devices-info-dialog.component';

enum SelectedDeviceState {
  ListSelected,
  PointsSelected,
  NoneSelcted
}

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css']
})
export class DeviceListComponent implements OnInit, OnDestroy {

  devices: Observable<Device[]>;
  selectedDevice: Device;
  selctedDeviceState: SelectedDeviceState;
  filter: string;
  Roles = Roles;
  private selectedDateRange: DateRange;
  private subscription: Subscription;

  constructor(
    private deviceService: DeviceService,
    private mapsService: MapsService,
    private toasterService: ToasterService,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.devices = this.deviceService.getDevices();

    this.showAllDevices();
  }

  ngOnDestroy() {
    this.unsubscribe();
  }

  showAllDevices() {
    this.subscription = this.deviceService.getLatestPoints()
      .subscribe(points => {
        const map = new Map<string, TrackingPoint>();
        for (const key of Object.keys(points)) {
          map.set(key, points[key]);
        }

        this.mapsService.showDevicesPositions(map);

        if (this.selectedDevice && !points[this.selectedDevice.name]) {
          this.toasterService.pop('error', '', 'Can\'t find position of ' + this.selectedDevice.name);
        } else if (this.selectedDevice) {
          this.mapsService.showDevice([this.selectedDevice.name, points[this.selectedDevice.name]]);
        }
      });
  }

  showDevice(device: Device) {
    if (device === this.selectedDevice && this.isDeviceListSelected()) {
      this.selectedDevice = null;
    } else {
      this.selectedDevice = device;
      this.selctedDeviceState = SelectedDeviceState.ListSelected;
      this.unsubscribe();
      this.showAllDevices();
      this.toasterService.pop('info', '', 'Now following device ' + this.selectedDevice.name);
    }
  }

  showPoints(device: Device, filterSelcted = false) {
    if (device === this.selectedDevice && this.isDevicePointSelected() && !filterSelcted) {
      this.selectedDevice = null;
      this.selctedDeviceState = SelectedDeviceState.NoneSelcted;
      this.unsubscribe();
      this.showAllDevices();

    } else {
      this.selectedDevice = device;
      this.selctedDeviceState = SelectedDeviceState.PointsSelected;
      this.toasterService.pop('info', '', 'Now showing latest points of ' + this.selectedDevice.name);
      this.unsubscribe();
      this.subscription = this.deviceService.getPoints(device.id, this.selectedDateRange)
        .subscribe(points => this.mapsService.showPoints(points));
    }
  }

  openDevicesDialog(): void {
    this.dialog.open(DevicesInfoDialogComponent, {  
      width: '600px',
    });
  }

  timeFilterChange(range) {
    this.selectedDateRange = range;
    this.showPoints(this.selectedDevice, true);
  }

  private unsubscribe() {
    if (this.subscription && !this.subscription.closed) {
      this.subscription.unsubscribe();
    }
  }

  isDeviceListSelected() {
    return this.selctedDeviceState === SelectedDeviceState.ListSelected;
  }

  isDevicePointSelected() {
    return this.selctedDeviceState === SelectedDeviceState.PointsSelected;
  }
}
