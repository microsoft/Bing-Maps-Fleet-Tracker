// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Point } from './point';
import { Location } from './location';

export class TripLeg {
    id: number;
    startLocationId: number;
    endLocationId: number;
    startLocation: Location;
    endLocation: Location;
    tripId: number;
    averageSpeed: number;
    startTimeUtc: string;
    endTimeUtc: string;
    route: Point[];
}
