// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage';

import 'rxjs/add/operator/map';

export enum Settings {
  BackendUrl,
  SecurityToken,
  BackgroundOptions
}

@Injectable()
export class SettingsService {

  storageReady;

  constructor(private storage: Storage) {
    this.storageReady = this.storage.ready();
  }

  set(key: Settings, value: any): Promise<void> {
    return this.storage.set(Settings[key], value);
  }

  get(key: Settings): Promise<any> {
    return this.storage.get(Settings[key]);
  }

  remove(key: Settings): Promise<any> {
    return this.storage.remove(Settings[key]);
  }

  clear(): Promise<any> {
    return this.storage.clear();
  }
}
