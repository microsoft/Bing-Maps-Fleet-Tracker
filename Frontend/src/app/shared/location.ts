// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Point } from './point';

export class Location extends Point {
    id: string;
    name: string;
    address: string;
    tags: string[];
}

export enum InterestLevel {
    Unknown = 0,
    AutoNew = 1,
    AutoLow = 2,
    AutoHigh = 30,
    Manual = 40
}
