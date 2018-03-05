// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/* tslint:disable:no-unused-variable */

import { TestBed, async } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { DataService } from './core/data.service';
import { MockDataService } from '../testing/mock-data.service';

import { AssetListComponent } from './assets/asset-list/asset-list.component';
import { AssetEditorComponent } from './assets/asset-editor/asset-editor.component';
import { AssetService } from './assets/asset.service';

import { DeviceListComponent } from './devices/device-list/device-list.component';
import { DeviceEditorComponent } from './devices/device-editor/device-editor.component';
import { DeviceService } from './devices/device.service';

import { BingMapsService } from './maps/bing-maps.service';
import { MapsComponent } from './maps/maps/maps.component';
import { MapsService } from './maps/maps.service';
import { MockBingMapsService } from '../testing/mock-bing-maps.service';


describe('AppComponent', () => {
  const dataService = new MockDataService();
  const bingMapsLoaderService = new MockBingMapsService();

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [
        AppComponent,
        AssetEditorComponent,
        AssetListComponent,
        DeviceEditorComponent,
        DeviceListComponent,
        MapsComponent
      ],
      providers: [
        AssetService,
        DeviceService,
        MapsService,
        {
          provide: BingMapsService,
          useValue: bingMapsLoaderService
        },
        {
          provide: DataService,
          useValue: dataService
        }
      ],
      imports: [
        FormsModule
      ]
    });
    TestBed.compileComponents();
  });

  it('should create the app', async(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  }));

  it('should render sidenav with child components', async(() => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('mat-sidenav')).toBeTruthy();
    expect(compiled.querySelector('h2').innerHTML).toBe('Trackable');
    expect(compiled.querySelector('app-asset-list')).toBeTruthy();
    expect(compiled.querySelector('app-device-list')).toBeTruthy();
    expect(compiled.querySelector('app-report-list')).toBeTruthy();
  }));

  it('should render mat-sidenav-content tag', async(() => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('mat-sidenav-content')).toBeTruthy();
  }));
});
