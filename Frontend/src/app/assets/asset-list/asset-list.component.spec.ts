/* tslint:disable:no-unused-variable */
import { Injectable } from '@angular/core';
import { By } from '@angular/platform-browser';
import { async, ComponentFixture, TestBed, inject } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs/Observable';

import { Asset, AssetType } from '../asset';
import { AssetEditorComponent } from '../asset-editor/asset-editor.component';
import { AssetListComponent } from './asset-list.component';
import { AssetService } from '../asset.service';
import { MapsService } from '../../maps/maps.service';

import 'hammerjs';
import 'rxjs/add/observable/of';

@Injectable()
export class MockAssetService {
  getAssets(): Observable<Asset[]> {
    return Observable.of([
      {
        id: 'bus99',
        trackingDeviceName: 'cairo phone',
        assetType: AssetType.Car,
        assetProperties: {}
      }
    ]);
  }
}

describe('AssetListComponent', () => {
  let component: AssetListComponent;
  let fixture: ComponentFixture<AssetListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AssetListComponent],
      providers: [
        MapsService,
        { provide: AssetService, useClass: MockAssetService }
      ],
      imports: [
        FormsModule
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
