// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Directive, ElementRef, Input, OnInit } from '@angular/core';
import { AuthService } from '../users/auth.service';
import { Roles, Role } from './role';

@Directive({ selector: '[appRoleLimiter]' })
export class RoleLimiterDirective implements OnInit {
    @Input() appRoleLimiter: Roles;
    constructor(private el: ElementRef, private authService: AuthService) {

    }

    ngOnInit(): void {
        this.authService.getLoggedInUser().subscribe(user => {
            if (user == null) {
                return;
            }

            if (this.appRoleLimiter > Roles[user.role.name]) {
                this.el.nativeElement.style.display = 'none';
            }
        });
    }
}
