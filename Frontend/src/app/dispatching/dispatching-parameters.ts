// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Point } from '../shared/point';

export class DispatchingParameters {
    assetId: string;
    getAlternativeCarRoute: boolean;
    wayPoints: Point[];
    maxSolutions: number;
    avoid: AvoidTypes[];
    distanceBeforeFirstTurn: number;
    heading: number;
    optimize: OptimizeValue;
    routeAttributes: RouteAttributes[];
    tolerances: number[];
    distanceUnit: DistanceUnit;
    weightUnit: WeightUnit;
    dimensionUnit: DimensionUnit;
    dateTime: Date;
    timeType: TimeType;
    loadedHeight: number;
    loadedWidth: number;
    loadedLength: number;
    loadedWeight: number;
    avoidCrossWind: boolean;
    avoidGroundingRisk: boolean;
    hazardousMaterials: HazardousMaterial[];
    hazardousPermits: HazardousMaterial[];
}

export enum AvoidTypes {
    Highways,
    Tolls,
    MinimizeHighways,
    MinimizeTolls
}

export enum OptimizeValue {
    Time,
    TimeWithTraffic
}

export enum RouteAttributes {
    ExcludeItinerary,
    RoutePath,
    RouteSummariesOnly,
    All
}

export enum DistanceUnit {
    Mile,
    Kilometer
}

export enum WeightUnit {
    Kilogram,
    Pound
}

export enum DimensionUnit {
    Meter,
    Foot
}

export enum TimeType {
    Arrival,
    Departure
}

export enum HazardousMaterial {
    Explosive,
    Gas,
    Flammable,
    Combustable,
    FlammableSolid,
    Organic,
    Poison,
    RadioActive,
    Corrosive,
    PoisonousInhalation,
    GoodsHarmfulToWater,
    AllApproppriateForLoad,
    Other
}
