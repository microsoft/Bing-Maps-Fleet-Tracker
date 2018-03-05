// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ToasterConfig } from 'angular2-toaster';
import { BodyOutputType } from 'angular2-toaster';

import { SpinnerService } from './core/spinner.service';
import { Roles } from './shared/role';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  Roles = Roles;
  public toasterconfig = new ToasterConfig(
    {
      limit: 1,
      positionClass: 'toast-padded-top-right'
    });

  constructor(
    private location: Location,
    public spinnerService: SpinnerService) { }

  ngOnInit(): void {
  }

  isReport() {
    return this.location.path() === '/reports';
  }
}
