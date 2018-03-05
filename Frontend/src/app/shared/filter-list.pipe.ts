// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filterList'
})
export class FilterListPipe implements PipeTransform {

  transform(items: any[], args?: any): any {
    let query = args;

    if (!query) {
      return items;
    }

    if (typeof query === 'string') {
      query = query.toLowerCase();
    }

    return items.filter(item => {
      for (const key in item) {
        if (item.hasOwnProperty(key)) {
          const val = item[key];
          switch (typeof val) {
            case 'string':
              if (val.toLowerCase().indexOf(query) > -1) { return true; }
              break;
            case 'number':
              if (val == query) { return true; }
              break;
          }
        }
      }
    });
  }
}
