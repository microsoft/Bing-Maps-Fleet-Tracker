// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { User } from '../../shared/user';
import { UserService } from '../user.service';
import { MatDialog } from '@angular/material/dialog';
import { UsersInfoDialogComponent } from '../users-info-dialog/users-info-dialog.component';


@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit, OnDestroy {

  users: Observable<User[]>;
  filter: string;

  constructor(
    private userService: UserService,
    public dialog: MatDialog) { }

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

  openUsersDialog(): void{
    this.dialog.open(UsersInfoDialogComponent, {  
      width: '600px',
    });
  }
}
