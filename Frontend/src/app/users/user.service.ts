import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Subject } from 'rxjs/Subject';

import { DataService } from '../core/data.service';
import { User } from '../shared/user';

@Injectable()
export class UserService {
  constructor(private dataService: DataService) { }

  public getAll() {
    return this.dataService.get<User>('users');
  }

  public post(user: User) {
    return this.dataService.post<User>('users', user);
  }

  public getToken(regenerate = false) {
    if (regenerate) {
      return this.dataService.post<string>('users/me/token', '', new Map([['regenerateToken', 'true']]));
    } else {
      return this.dataService.post<string>('users/me/token', '');
    }
  }

  public get(id: string) {
    return this.dataService.getSingle<User>('users', id);
  }

  public put(user: User) {
    return this.dataService.put<User>('users', user.id, user, true);
  }

  public delete(user: User) {
    return this.dataService.delete<User>('users', user.id);
  }
}
