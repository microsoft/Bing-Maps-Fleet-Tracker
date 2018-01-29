import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';

@Injectable()
export class MockDataService {

    private data = {};

    constructor() { }

    mock<T>(path: string, data: T) {
        this.data[path] = data;
    }

    get<T>(path: string): Observable<T> {
        return Observable.of(this.data[path] as T);
    }

    post<T>(path: string, data: T) {
    }

    put<T>(path: string, id: string | number, data: T, updateAll: boolean) {
    }
}
