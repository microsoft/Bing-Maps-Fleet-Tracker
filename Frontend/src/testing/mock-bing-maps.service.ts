// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';

@Injectable()
export class MockBingMapsService {

    constructor() {
    }

    init(element: HTMLElement, options: Microsoft.Maps.IMapLoadOptions): void {
    }
}
