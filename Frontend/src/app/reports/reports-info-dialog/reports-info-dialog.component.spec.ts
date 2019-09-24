// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsInfoDialogComponent } from './reports-info-dialog.component';

describe('ReportsInfoDialogComponent', () => {
  let component: ReportsInfoDialogComponent;
  let fixture: ComponentFixture<ReportsInfoDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportsInfoDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsInfoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
