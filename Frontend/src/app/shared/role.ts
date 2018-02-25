export enum Roles {
    Blocked = 0,
    Pending = 10,
    DeviceRegistration = 13,
    TrackingDevice = 16,
    Viewer = 20,
    Administrator = 30,
    Owner = 40
}

export class Role {
    name: string;

    constructor(name = '') {
        this.name = name;
    }
}
