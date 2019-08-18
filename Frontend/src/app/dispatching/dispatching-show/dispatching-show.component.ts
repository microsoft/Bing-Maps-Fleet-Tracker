// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit } from '@angular/core';

import { DispatchingService } from '../dispatching.service';
import { MapsService } from '../../maps/maps.service';
import { SpinnerService } from '../../core/spinner.service';
import { ToasterService } from 'angular2-toaster';

import { Point } from '../../shared/point';
import { Location } from '../../shared/location';

interface Route {
  directions: string[]
  distances: string[]
  directionPoints: Point[]
  routePoint: Point[]
}

@Component({
  selector: 'app-dispatching-show',
  templateUrl: './dispatching-show.component.html',
  styleUrls: ['./dispatching-show.component.css']
})
export class DispatchingShowComponent implements OnInit {

  mainRoute: Route = {
    directions: [],
    distances: [],
    directionPoints: [],
    routePoint: []
  };
  altRoute: Route = {
    directions: [],
    distances: [],
    directionPoints: [],
    routePoint: []
  };
  viewAltRoute: boolean;
  noDirectionsAvailable: boolean;
  location: Location;

  constructor(
    private dispatchingService: DispatchingService,
    private mapService: MapsService,
    private spinnerService: SpinnerService,
    private toasterService: ToasterService) {
    this.spinnerService.start();
  }

  ngOnInit() {
    this.location = new Location();
    this.dispatchingService.getDispatchingResults()
      .subscribe(results => {
        this.viewAltRoute = this.dispatchingService.getDispatchingParameters().getAlternativeCarRoute;

        this.mainRoute = {
          directions: results[0].itineraryText,
          distances: results[0].itineraryDistance,
          directionPoints: results[0].itineraryPoints,
          routePoint: results[0].routePoints,
        }
        if (results[0].alternativeCarRoutePoints != null) {
          this.altRoute = {
            directions: results[0].alternativeCarRoutePoints[0].itineraryText,
            distances: results[0].alternativeCarRoutePoints[0].itineraryDistance,
            directionPoints: results[0].alternativeCarRoutePoints[0].itineraryPoints,
            routePoint: results[0].alternativeCarRoutePoints[0].routePoints,
          }
          this.renameDestinationsInAltDirections();
        }

        this.noDirectionsAvailable = this.mainRoute.directions.length === 0;

        this.renameDestinationsInDirections();

        this.mapService.showAlternativeResults(this.altRoute.routePoint);
        this.mapService.showDispatchingResults(this.mainRoute.routePoint, this.dispatchingService.getPinsAdded());

        this.spinnerService.stop();
      }, error => {
        this.toasterService.pop('error', 'Error routing',
          'An error has occured. Please make sure that the locations you are trying to route to are in the supported regions');
      });


  }

  toggleRoutes(item) {
    if (item.tab.textLabel === "Main Route") {
      this.mapService.showAlternativeResults(this.altRoute.routePoint);
      this.mapService.showDispatchingResults(this.mainRoute.routePoint, this.dispatchingService.getPinsAdded());

    } else {
      this.mapService.showAlternativeResults(this.mainRoute.routePoint);
      this.mapService.showDispatchingResults(this.altRoute.routePoint, this.dispatchingService.getPinsAdded());

    }
  }

  private renameDestinationsInDirections() {
    this.dispatchingService.getDispatchingPinsResult()
      .subscribe(location => {
        var directions = this.mainRoute.directions;
        var pinIndex = 1;
        for (var i = 0; i < directions.length && location.length > 0; i++) {
          var direction = directions[i];
          if (direction.startsWith("Arrive at Stop: Y")) {
            directions[i] = "Arrive at Stop " + pinIndex + ": " + location[pinIndex].address
            pinIndex += 1;
          }
        }
        this.mainRoute.directions = directions;
      });
  }

  private renameDestinationsInAltDirections() {
    this.dispatchingService.getDispatchingPinsResult()
      .subscribe(location => {
        var directions = this.altRoute.directions;
        var pinIndex = 1;
        for (var i = 0; i < directions.length && location.length > 0; i++) {
          var direction = directions[i];
          if (direction.startsWith("Arrive at Stop: Y")) {
            directions[i] = "Arrive at Stop " + pinIndex + ": " + location[pinIndex].address
            pinIndex += 1;
          }
        }
        this.altRoute.directions = directions;
      });


  }

  showItineraryPoint(p) {
    this.mapService.showItineraryPosition(p);
  }
}
