// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

export class Metric {
    name: string;
    units: string;
    aggregatedBy: string;
    values: Map<string, string>;
}
