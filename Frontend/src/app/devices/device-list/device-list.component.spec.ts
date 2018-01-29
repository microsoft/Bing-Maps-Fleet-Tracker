/* tslint:disable:no-unused-variable */
import { Injectable } from '@angular/core';
import { By } from '@angular/platform-browser';
import { async, ComponentFixture, TestBed, inject } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs/Observable';

import { Device } from '../device';
import { DeviceEditorComponent } from '../device-editor/device-editor.component';
import { DeviceListComponent } from './device-list.component';
import { DeviceService } from '../device.service';
import { MapsService } from '../../maps/maps.service';

import 'hammerjs';
import 'rxjs/add/observable/of';

@Injectable()
export class MockDeviceService {
  getDevices(): Observable<Device[]> {
    return Observable.of([
      {
        id: 'cairo phone',
        assetId: 'bus66'
      }
    ]);
  }
}

describe('DeviceListComponent', () => {
  let component: DeviceListComponent;
  let fixture: ComponentFixture<DeviceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DeviceListComponent, DeviceEditorComponent],
      providers: [
        MapsService,
        { provide: DeviceService, useClass: MockDeviceService }
      ],
      imports: [
        FormsModule
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeviceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
