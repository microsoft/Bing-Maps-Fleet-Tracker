// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { AssetProperties } from '../shared/asset-properties';

export class Asset {
    id: string;
    trackingDeviceName?: string;
    trackingDeviceId?: string;
    assetType: AssetType;
    assetProperties: AssetProperties;
}

export enum AssetType {
    Car,
    Truck
}
