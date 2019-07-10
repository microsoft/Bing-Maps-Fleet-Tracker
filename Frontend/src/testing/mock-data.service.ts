
import {of as observableOf,  Observable } from 'rxjs';
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';


@Injectable()
export class MockDataService {

    private data = {};

    constructor() { }

    mock<T>(path: string, data: T) {
        this.data[path] = data;
    }

    get<T>(path: string): Observable<T> {
        return observableOf(this.data[path] as T);
    }

    post<T>(path: string, data: T) {
    }

    put<T>(path: string, id: string | number, data: T, updateAll: boolean) {
    }
}
