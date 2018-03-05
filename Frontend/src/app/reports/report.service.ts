// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';

import { Metric } from './metric';
import { DataService } from '../core/data.service';

@Injectable()
export class ReportService {

  selectedMetric: Subject<Metric>;

  constructor(private dataService: DataService) {
    this.selectedMetric = new Subject();
  }

  getAllMetrics(): Observable<Metric[]> {
    return this.dataService.get<Metric>('reports');
  }

  getSelectedMetric(): Observable<Metric> {
    return this.selectedMetric.asObservable();
  }

  setSelectedMetric(metric: Metric) {
    this.selectedMetric.next(metric);
  }
}
