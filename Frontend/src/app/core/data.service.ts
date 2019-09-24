// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Cache } from './cache';
import { AuthorizedHttpService } from './authorized-http.service';
import { SpinnerService } from './spinner.service';
import { map } from 'rxjs/operators';

import { EnvironmentSettings, EnvironmentSettingsService } from './environment-settings.service';


@Injectable()
export class DataService {

    private cacheMap: Map<string, Cache<any>>;

    constructor(
        private environmentSettingsService: EnvironmentSettingsService,
        private authHttpService: AuthorizedHttpService,
        private spinnerService: SpinnerService) {
        this.cacheMap = new Map<string, Cache<any>>();
    }

    get<T>(path: string, internalRequest: boolean = true): Observable<T[]> {
        this.spinnerService.start();
        const cache = this.getCache<T>(path);
        var url = path
        if (internalRequest) {
            url = this.getUrl(path);
        }

        this.authHttpService.get(url).pipe(
            map(response => response as unknown as T[]))
            .subscribe(data => {
                cache.set(data);
                this.spinnerService.stop();
            },
                error => this.spinnerService.stop());

        return cache.getItems();
    }

    getNoCache<T>(path: string, internalRequest: boolean = true, withCredentials: boolean = true): Observable<T[]> {
        var url = path
        if (internalRequest) {
            url = this.getUrl(path);
        }

        const observable = this.authHttpService.get<T[]>(url, withCredentials);

        return observable;
    }

    getSingleNoCache<T>(path: string): Observable<T> {
        const url = this.getUrl(path);

        const observable = this.authHttpService.get<T>(url);

        return observable;
    }

    getSingle<T>(path: string, id?: string | number): Observable<T> {
        const cache = this.getCache(path);
        const url = this.getUrl(path, id);
        const subject = new BehaviorSubject<T>(cache.get(id) as T);

        this.authHttpService.get<T>(url)
            .subscribe(data => { cache.update(data); subject.next(data); }, error => subject.error(error));

        return subject.asObservable();
    }

    post<T>(path: string, data: T, queryParameters: Map<string, string> = null): Observable<any> {
        const cache = this.getCache(path);
        let url = this.getUrl(path);

        if (queryParameters != null) {
            queryParameters.forEach((value: string, key: string) => {
                if (key === queryParameters.keys().next().value) {
                    url = url.concat('?' + key + '=' + value);
                } else {
                    url = url.concat('&' + key + '=' + value);
                }
            });
        }

        const observable = this.authHttpService.post(url, data).pipe(
            map(response => typeof response != undefined ? response : null));

        observable
            .subscribe(d => cache.add(d), error => { });

        return observable;
    }

    put<T>(path: string, id: string | number, data: T, updateAll: boolean): Observable<any> {
        const cache = this.getCache(path);
        const url = this.getUrl(path, id);
        const observable = this.authHttpService.put(url, data).pipe(map(response => typeof response != undefined ? response : null));

        observable
            .subscribe(d => {
                if (updateAll) {
                    this.get<T>(path);
                } else {
                    cache.update(d);
                }
            });

        return observable;
    }

    delete<T>(path: string, id: string | number): Observable<any> {
        const cache = this.getCache(path);
        const url = this.getUrl(path, id);
        const observable = this.authHttpService.delete(url);

        observable
            .subscribe(d => this.get<T>(path));

        return observable;
    }

    private getUrl(path: string, id?: string | number) {
        const url = `${this.environmentSettingsService.getEnvironmentVariable(EnvironmentSettings.backendUrl)}/${path}`;
        return id ? `${url}/${id}` : url;
    }

    private getCache<T>(path): Cache<T> {
        if (!this.cacheMap.has(path)) {
            this.cacheMap.set(path, new Cache());
        }

        return this.cacheMap.get(path);
    }
}
