// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TestBed, inject } from '@angular/core/testing';

import { DispatchingService } from './dispatching.service';

describe('DispatchingService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DispatchingService]
    });
  });

  it('should ...', inject([DispatchingService], (service: DispatchingService) => {
    expect(service).toBeTruthy();
  }));
});
