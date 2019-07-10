// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable ,  BehaviorSubject ,  Subject } from 'rxjs';
import { EnvironmentSettings, EnvironmentSettingsService } from '../core/environment-settings.service';

import { DataService } from '../core/data.service';
import { User } from '../shared/user';

@Injectable()
export class AuthService {

  loggedInUser: BehaviorSubject<User> = null;

  constructor(private dataService: DataService,
    private envSettingsService: EnvironmentSettingsService) {  }

  public getLoggedInUser() {
    if (this.loggedInUser == null) {
      this.loggedInUser = new BehaviorSubject(null);
      this.dataService.getSingle<User>('users', 'me')
        .subscribe(a => this.loggedInUser.next(a), e => this.loggedInUser.error(e));
    }
    return this.loggedInUser.asObservable();
  }

  public loginOrGetUser() {
    const observable = this.getLoggedInUser();
    observable.subscribe(d => {},
      e => {
        if (e.status === 401) {
          this.login();
        }
      });

    return observable;
  }

  public logout() {
    window.location.href = this.getLogoutUrl();
  }

  public login() {
    window.location.href = this.getLoginUrl();
  }

  public getLogoutUrl() {
    return `${this.envSettingsService.getEnvironmentVariable(EnvironmentSettings.backendUrl)}/`
      + `users/logout?redirectUri=${this.envSettingsService.getEnvironmentVariable(EnvironmentSettings.frontendUrl)}`;
  }

  public getLoginUrl(): string {
    return `${this.envSettingsService.getEnvironmentVariable(EnvironmentSettings.backendUrl)}/`
      + `users/login?redirectUri=${this.envSettingsService.getEnvironmentVariable(EnvironmentSettings.frontendUrl)}`;
  }
}
