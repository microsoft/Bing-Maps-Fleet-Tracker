// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit } from '@angular/core';
import { DispatchingService } from '../dispatching.service';
import { MapsService } from '../../maps/maps.service';
import { SpinnerService } from '../../core/spinner.service';
import { ToasterService } from 'angular2-toaster';
import { Point } from '../../shared/point';
import { ShowOnDirtyErrorStateMatcher } from '../../../../node_modules/@angular/material';

@Component({
  selector: 'app-dispatching-show',
  templateUrl: './dispatching-show.component.html',
  styleUrls: ['./dispatching-show.component.css']
})
export class DispatchingShowComponent implements OnInit {

  directions: string[];
  distances: string[];
  directionPoints: Point[];
  noDirectionsAvailable: boolean;

  constructor(
    private dispatchingService: DispatchingService,
    private mapService: MapsService,
    private spinnerService: SpinnerService,
    private toasterService: ToasterService) {
      this.spinnerService.start();
    }

  ngOnInit() {
    this.dispatchingService.getDispatchingResults()
      .subscribe(results => {
        this.directions = results[0].itineraryText;
        this.directionPoints = results[0].itineraryPoints;
        this.distances = results[0].itineraryDistance;
        this.noDirectionsAvailable = this.directions.length === 0;
        this.mapService.showAlternativeResults(results[0].alternativeCarRoutePoints);
        this.mapService.showDispatchingResults(results[0].routePoints, this.dispatchingService.getPinsAdded());
        this.spinnerService.stop(); }, error => {
        this.toasterService.pop('error', 'Error routing',
        'An error has occured. Please make sure that the locations you are trying to route to are in the supported regions');
      });
  }

  showItineraryPoint(index: number) {
    this.mapService.showItineraryPosition(this.directionPoints[index]);
  }

  pushNotification(){
        if(!this.noDirectionsAvailable)

      this.dispatchingService.callSignalRAPI()
      .subscribe(result => {
        if(result == 0){
          this.toasterService.pop('info', '','Successfully Dispatched to Mobile');
        } else if (result == 1) {
          this.toasterService.pop('info', '', 'Successfully Dispatched to Mobile , Mobile is currently offline');
         } else if (result == 2) {
          this.toasterService.pop('info', '', 'Failed to Dispatch to Mobile');
         }
      });
  }
}