import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IMyOptions, IMyDate, IMyDateRange, IMyDateRangeModel } from 'mydaterangepicker';

import { DateRange } from '../date-range';

@Component({
  selector: 'app-time-filter',
  templateUrl: './time-filter.component.html',
  styleUrls: ['./time-filter.component.css']
})
export class TimeFilterComponent implements OnInit {

  @Output() change: EventEmitter<DateRange> = new EventEmitter();
  selectedRange: DateRange;
  selectedDateRange: IMyDateRange;
  ranges: DateRange[] = [];

  myDateRangePickerOptions: IMyOptions = {
    dateFormat: 'dd.mm.yyyy',
  };

  constructor() { }

  ngOnInit() {
    const today = new Date();
    const daysToMilliSeconds = 1000 * 60 * 60 * 24;

    this.ranges = [
      { label: 'One Day', from: new Date(today.getTime() - (1 * daysToMilliSeconds)), to: today },
      { label: 'Three Days', from: new Date(today.getTime() - (3 * daysToMilliSeconds)), to: today },
      { label: 'One Week', from: new Date(today.getTime() - (7 * daysToMilliSeconds)), to: today },
      { label: 'Two Weeks', from: new Date(today.getTime() - (14 * daysToMilliSeconds)), to: today },
      { label: 'One Month', from: new Date(today.getTime() - (31 * daysToMilliSeconds)), to: today },
      { label: 'Two Months', from: new Date(today.getTime() - (62 * daysToMilliSeconds)), to: today }
    ];
  }

  select(range: DateRange): void {
    this.selectedRange = range;

    this.selectedDateRange = {
      beginDate: this.convertDate(range.from),
      endDate: this.convertDate(range.to)
    };

    this.change.emit(range);
  }

  onDateRangeChanged(event: IMyDateRangeModel) {
    this.selectedRange = null;
    this.change.emit({
      from: event.beginJsDate,
      to: event.endJsDate,
      label: ''
    });
  }

  private convertDate(date: Date): IMyDate {
    return {
      day: date.getDate(),
      month: date.getMonth() + 1,
      year: date.getFullYear()
    };
  }
}
