// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersInfoDialogComponent } from './users-info-dialog.component';

describe('UsersInfoDialogComponent', () => {
  let component: UsersInfoDialogComponent;
  let fixture: ComponentFixture<UsersInfoDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsersInfoDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsersInfoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
