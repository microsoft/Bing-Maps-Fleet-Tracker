import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { User } from '../../shared/user';
import { UserService } from '../user.service';


@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit, OnDestroy {

  users: Observable<User[]>;
  filter: string;

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.users = this.userService.getAll();
  }

  ngOnDestroy() {
  }

  deleteUser(user: User) {
    if (confirm(`Are you sure you want to delete ${user.name} with email ${user.email}?`)) {
      this.userService.delete(user);
    }
  }
}
