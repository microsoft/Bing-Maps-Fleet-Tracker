// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Point } from '../shared/point';

export class DispatchingResults {
    itineraryText: string[];
    itineraryPoints: Point[];
    itineraryDistance: string[];
    routePoints: Point[];
    alternativeCarRoutePoints: DispatchingResults[];
}
