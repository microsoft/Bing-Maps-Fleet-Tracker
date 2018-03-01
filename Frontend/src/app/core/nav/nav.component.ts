// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Roles } from '../../shared/role';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
  opened = true;
  Roles = Roles;
  @Output() toggle = new EventEmitter();

  constructor() { }

  onToggle(): void {
    this.opened = !this.opened;
    this.toggle.emit();
  }
}
