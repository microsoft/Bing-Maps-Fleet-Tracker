// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class SpinnerService {

  spinning = false;

  constructor() { }

  start() {
      this.spinning = true;
  }

  stop() {
      this.spinning = false;
  }
}
