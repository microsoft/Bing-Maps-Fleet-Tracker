// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { HttpClient } from '@angular/common/http';
import { Observable ,  BehaviorSubject ,  Subject } from 'rxjs';


export class Cache<T> {

    private subject: BehaviorSubject<T[]>;
    private items: T[] = [];

    constructor() {
        this.subject = new BehaviorSubject([]);
    }

    getItems(): Observable<T[]> {
        return this.subject.asObservable();
    }

    set(data: T[]) {
        this.items = data;
        this.notify();
    }

    add(item: T) {
        this.items.push(item);
        this.notify();
    }

    get(id: string | number): T {
        for (const item of this.items) {
            if (item['id'] === id) {
                return item;
            }
        }

        return null;
    }

    update(item: T) {
        for (let i = 0; i < this.items.length; i++) {
            if (this.items[i]['id'] === item['id']) {
                this.items[i] = item;
                this.notify();
                return;
            }
        }
    }

    notify() {
        this.subject.next(Object.assign([], this.items));
    }
}
