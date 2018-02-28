import { Point } from './point';

export class Geofence {
    id: number;
    name: string;
    emailsToNotify: string[];
    webhooksToNotify: string[];
    cooldown: number;
    fenceType: string;
    fencePolygon: Point[];
    fenceCenter: Point;
    radiusInMeters: number;
    areaType: AreaType;
    assetIds: string[];
}

export enum FenceType {
    Inbound,
    Outbound
}

export enum AreaType {
    Unknown,
    Polygon,
    Circular
}
