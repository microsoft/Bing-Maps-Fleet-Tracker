import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { Metric } from '../metric';
import { ReportService } from '../report.service';


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

  constructor(private reportService: ReportService) { }

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
}
