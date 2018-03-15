// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { AssetProperties } from '../shared/asset-properties';
import { Device } from '../devices/device';

export class Asset {
    id: string;
    name: string;
    trackingDeviceId?: string;
    trackingDevice?: Device;
    assetType: AssetType;
    assetProperties: AssetProperties;
}

export enum AssetType {
    Car = <any>'Car',
    Truck = <any>'Truck'
}
