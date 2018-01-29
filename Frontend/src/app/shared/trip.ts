import { TripLeg } from './trip-leg';
import { Location } from './location';

export class Trip {
    id: number;
    startLocationId: number;
    endLocationId: number;
    startLocation: Location;
    endLocation: Location;
    assetId: string;
    trackingDeviceId: string;
    startTimeUtc: string;
    endTimeUtc: string;
    tripLegs: TripLeg[];
    durationInMinutes: number;
    startTimeStampUtc: number;
}
