// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { ToasterService } from 'angular2-toaster';

@Injectable()
export class AuthorizedHttpService {
    constructor(
        private http: HttpClient,
        private toasterService: ToasterService) {
    }

    public get<T>(path: string, withCredentials: boolean = true): Observable<T> {
        const subject = new Subject<any>();

        this.http.get<T>(path, { withCredentials: withCredentials })
            .subscribe(
                data => subject.next(data),
                error => {
                    this.authErrorHandler(error);
                    subject.error(error);
                });

        return subject.asObservable();
    }

    public put(path: string, dataToUse): Observable<Response> {
        const subject = new Subject<any>();

        this.http.put(path, dataToUse, { withCredentials: true })
            .subscribe(data => subject.next(data), error => {
                this.authErrorHandler(error);
                subject.error(error);
            });

        return subject.asObservable();
    }

    public post(path: string, dataToUse): Observable<Response> {
        const subject = new Subject<any>();

        this.http.post(path, dataToUse, { withCredentials: true })
            .subscribe(data => subject.next(data), error => {
                this.authErrorHandler(error);
                subject.error(error);
            });

        return subject.asObservable();
    }

    public delete(path: string): Observable<Response> {
        const subject = new Subject<any>();

        this.http.delete(path, { withCredentials: true })
            .subscribe(data => subject.next(data), error => {
                this.authErrorHandler(error);
                subject.error(error);
            });

        return subject.asObservable();
    }

    private authErrorHandler(error) {
        if (error.status === 401) {
            this.toasterService.pop('info', 'Not logged in', 'Please login to be able to access the service');
        } else if (error.status === 403) {
            this.toasterService.pop('error', 'Insufficient permissions', 'You are not allowed to perform this action');
        } else {
            this.toasterService.pop('error', 'Error Occured', 'An error occured while contacting the server');
        }
    }
}
