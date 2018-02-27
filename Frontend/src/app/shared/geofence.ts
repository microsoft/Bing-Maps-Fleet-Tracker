import { Point } from './point';

export class Geofence {
    id: number;
    name: string;
    emailsToNotify: string[];
    webhooksToNotify: string[];
    cooldown: number;
    fenceType: string;
    fencePolygon: Point[];
    assetIds: string[];
}

export enum FenceType {
    Inbound,
    Outbound
}
