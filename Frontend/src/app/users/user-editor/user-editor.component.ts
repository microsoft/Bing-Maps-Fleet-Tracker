import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { ToasterService } from 'angular2-toaster';

import { User } from '../../shared/user';
import { Role } from '../../shared/role';
import { UserService } from '../user.service';

@Component({
  selector: 'app-user-editor-dialog',
  templateUrl: './user-editor.component.html',
  styleUrls: ['./user-editor.component.css']
})

export class UserEditorComponent implements OnInit, OnDestroy {
  user: User;
  isAddingNew: Boolean;
  roles = [new Role('Administrator'), new Role('Viewer'), new Role('Pending'), new Role('Blocked')];
  userToken = '';

  private routerSubscription: Subscription;
  private userSubscription: Subscription;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private toasterService: ToasterService) {
    this.user = new User();
    this.user.role = new Role();
  }

  ngOnInit() {
    this.routerSubscription = this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.isAddingNew = false;
        this.userSubscription = this.userService.get(id).subscribe(user => this.user = user);
      } else {
        this.isAddingNew = true;
      }
    });

    this.userService.getToken().subscribe(t => this.userToken = t);
  }

  ngOnDestroy() {
    this.routerSubscription.unsubscribe();

    if (this.userSubscription && !this.userSubscription.closed) {
      this.userSubscription.unsubscribe();
    }
  }

  regenerateToken() {
    this.userService.getToken(true).subscribe(t => {
      this.userToken = t;
      this.toasterService.pop('success', 'Token Regenerated');
    });
  }

  submit() {
    if (this.isAddingNew) {
      this.userService.post(this.user)
        .subscribe(() => this.router.navigate(['/users']));

    } else {
      this.userService.put(this.user)
        .subscribe(() => this.router.navigate(['/users']));
    }
  }
}
