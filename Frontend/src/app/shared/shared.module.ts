// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatOptionModule, MatLineModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
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
