// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Point } from './point';
import { Location } from './location';

export class TripLeg {
    startLocation: Location;
    endLocation: Location;
    averageSpeed: number;
    startTimeUtc: string;
    endTimeUtc: string;
    route: Point[];
}
