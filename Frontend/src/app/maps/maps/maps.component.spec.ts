// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BingMapsService } from '../bing-maps.service';
import { MapsComponent } from './maps.component';
import { MapsService } from '../maps.service';
import { MockBingMapsService } from '../../../testing/mock-bing-maps.service';

describe('MapsComponent', () => {
  let component: MapsComponent;
  let fixture: ComponentFixture<MapsComponent>;
  const bingMapsLoaderService = new MockBingMapsService();

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [MapsComponent],
      providers: [
        MapsService,
        {
          provide: BingMapsService,
          useValue: bingMapsLoaderService
        }],
      imports: [
        FormsModule
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MapsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
