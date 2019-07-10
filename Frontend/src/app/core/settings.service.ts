
import {map} from 'rxjs/operators';
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { LocationStrategy } from '@angular/common';
import { Observable } from 'rxjs';

import { DataService } from './data.service';
import { VersionInfo } from './version-info';
import { SubscriptionKey } from './subscription-key';

export enum SubscriptionKeys {
    BingMaps,
    Johannesburg
}

@Injectable()
export class SettingsService {
    constructor(private dataService: DataService) {
    }

    getSubscriptionKeys() {
        return this.dataService.get<SubscriptionKey>('settings/subscriptionkeys');
    }

    getSubscriptionKey(key: SubscriptionKeys): Observable<SubscriptionKey> {
        return this.dataService
            .getNoCache<SubscriptionKey>('settings/subscriptionkeys').pipe(
            map(results => results[key.toString()]));
    }

    getVersionInfo() {
        return this.dataService.getSingleNoCache<VersionInfo>(`settings/version`);
    }

    getInstrumentationApproval() {
        return this.dataService.getSingleNoCache<boolean | null>(`settings/instrumentation`);
    }

    setInstrumentationApproval(approval: boolean) {
        return this.dataService.post(`settings/instrumentation?approval=${approval}`, '');
    }
}
