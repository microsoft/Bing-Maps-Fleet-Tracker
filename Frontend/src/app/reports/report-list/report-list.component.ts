// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { Metric } from '../metric';
import { ReportService } from '../report.service';
import { MatDialog } from '@angular/material/dialog';
import { ReportsInfoDialogComponent } from '../reports-info-dialog/reports-info-dialog.component';


@Component({
  selector: 'app-report-list',
  templateUrl: './report-list.component.html',
  styleUrls: ['./report-list.component.css']
})
export class ReportListComponent implements OnInit, OnDestroy {

  metrics: Observable<Metric[]>;
  metricsSubscription: Subscription;
  filter: string;

  selectedMetric: Metric;

  constructor(private reportService: ReportService,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.metrics = this.reportService.getAllMetrics();
    this.metricsSubscription = this.metrics.subscribe(values => {
      if (values.length > 0) {
        this.selectMetric(values[0]);
      }
    });
  }

  ngOnDestroy() {
    this.metricsSubscription.unsubscribe();
  }

  selectMetric(metric: Metric) {
    this.selectedMetric = metric;
    this.reportService.setSelectedMetric(metric);
  }

  openReportsDialog(): void {
    this.dialog.open(ReportsInfoDialogComponent, {  
      width: '600px',
    });
  }
}
