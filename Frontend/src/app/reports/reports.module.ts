// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { ChartsModule } from 'ng2-charts';
import { ReportsRoutingModule, reportsRoutedComponents } from './reports-routing.module';
import { ReportService } from './report.service';
import { ReportChartComponent } from './report-chart/report-chart.component';
import { ReportsInfoDialogComponent } from './reports-info-dialog/reports-info-dialog.component';


@NgModule({
  declarations: [
    reportsRoutedComponents,
    ReportChartComponent,
    ReportsInfoDialogComponent
  ],
  imports: [
    ReportsRoutingModule,
    SharedModule,
    ChartsModule
  ],
  entryComponents: [
    ReportsInfoDialogComponent
  ],
  providers: [
    ReportService
  ],
  exports: [
    ReportChartComponent
  ]
})
export class ReportsModule { }
