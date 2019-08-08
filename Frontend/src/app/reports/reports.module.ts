// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { ReportsRoutingModule, reportsRoutedComponents } from './reports-routing.module';
import { ReportService } from './report.service';
import { ReportChartComponent } from './report-chart/report-chart.component';


@NgModule({
  declarations: [
    reportsRoutedComponents,
    ReportChartComponent
  ],
  imports: [
    ReportsRoutingModule,
    SharedModule,
    ChartsModule
  ],
  providers: [
    ReportService
  ],
  exports: [
    ReportChartComponent
  ]
})
export class ReportsModule { }
