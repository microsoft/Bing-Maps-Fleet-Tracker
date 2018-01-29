
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSidenavModule, MatInputModule, MatIconModule, MatRadioModule, MatListModule, MatSelectModule, MatToolbarModule,
         MatOptionModule, MatCheckboxModule, MatTooltipModule, MatButtonModule, MatLineModule, MatDialogModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Routes, RouterModule } from '@angular/router';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import { NgxPaginationModule } from 'ngx-pagination';
import { NguiDatetimePickerModule } from '@ngui/datetime-picker';
import { ToasterService } from 'angular2-toaster';

import { TimeFilterComponent } from './time-filter/time-filter.component';
import { TripListComponent } from './trip-list/trip-list.component';
import { FilterListPipe } from './filter-list.pipe';

import { RoleLimiterDirective } from './role-limiter.directive';

import 'hammerjs';

@NgModule({
  declarations: [
    FilterListPipe,
    TimeFilterComponent,
    TripListComponent,
    RoleLimiterDirective
    ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MatSidenavModule, MatInputModule, MatIconModule, MatRadioModule, MatListModule, MatSelectModule, MatToolbarModule,
    MatOptionModule, MatCheckboxModule, MatTooltipModule, MatButtonModule, MatLineModule, MatDialogModule,
    RouterModule,
    MyDateRangePickerModule,
    NgxPaginationModule,
    NguiDatetimePickerModule
  ],
  exports: [
    BrowserModule,
    FormsModule,
    MatSidenavModule, MatInputModule, MatIconModule, MatRadioModule, MatListModule, MatSelectModule, MatToolbarModule,
    MatOptionModule, MatCheckboxModule, MatTooltipModule, MatButtonModule, MatLineModule, MatDialogModule,
    FilterListPipe,
    TimeFilterComponent,
    TripListComponent,
    RoleLimiterDirective,
    NgxPaginationModule,
    NguiDatetimePickerModule
  ],
  providers: [
    ToasterService
  ]
})
export class SharedModule { }
