import { NgModule } from '@angular/core';
import { Routes, RouterModule, CanActivate } from '@angular/router';

 import { ReportListComponent } from './report-list/report-list.component';

const routes: Routes = [
  {
    path: 'reports', component: ReportListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }

export const reportsRoutedComponents = [
  ReportListComponent,
];