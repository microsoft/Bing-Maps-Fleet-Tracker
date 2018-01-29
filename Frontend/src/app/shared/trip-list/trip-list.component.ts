import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { Trip } from '../trip';

@Component({
  selector: 'app-trip-list',
  templateUrl: './trip-list.component.html',
  styleUrls: ['./trip-list.component.css']
})
export class TripListComponent implements OnInit {

  @Input() trips: Trip[] = [];
  @Output() select: EventEmitter<Trip> = new EventEmitter();

  selectedTrip: Trip;

  constructor() { }

  ngOnInit() {
  }

  selectTrip(trip) {
    this.selectedTrip = trip;
    this.select.emit(trip);
  }
}
