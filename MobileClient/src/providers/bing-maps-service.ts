// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Injectable } from '@angular/core';
import { Logger } from 'angular2-logger/core';
import 'rxjs/add/operator/map';

@Injectable()
export class BingMapsService {
  private loadPromise: Promise<void>;

  constructor(private logger: Logger) {
    this.logger.info('Hello BingMapsService Provider');
  }

  load(): Promise<void> {
    if (this.loadPromise) {
      return this.loadPromise;
    }

    let script = document.createElement('script');
    script.type = 'text/javascript';
    script.async = true;
    script.defer = true;

    let mapsCallback = 'bingMapsCallback'
    script.src = `http://www.bing.com/api/maps/mapcontrol?branch=release&clientApi=bingmapsfleettracker&callback=${ mapsCallback }`;

    this.loadPromise = new Promise<void>((resolve: Function, reject: Function) => {
      window[mapsCallback] = () => {
        this.logger.info('inside maps callback');
        resolve();
      };
      script.onerror = (error: Event) => { 
        this.logger.error('maps script error' + error);
        reject(error);
      };
    });

    document.body.appendChild(script);

    return this.loadPromise;
  }

  createMap(element: HTMLElement, options: Microsoft.Maps.IMapLoadOptions): Promise<Microsoft.Maps.Map> {
    return this.load().then(() => {
      this.logger.info('about to create map');
      return new Microsoft.Maps.Map(element, options);
    });
  }
}
