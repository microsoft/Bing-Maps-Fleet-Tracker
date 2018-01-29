import { TestBed, inject } from '@angular/core/testing';

import { LocationService } from './location.service';

describe('LocationService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LocationService]
    });
  });

  it('should ...', inject([LocationService], (service: LocationService) => {
    expect(service).toBeTruthy();
  }));
});
