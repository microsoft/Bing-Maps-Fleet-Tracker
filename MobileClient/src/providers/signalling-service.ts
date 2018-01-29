import { Injectable } from '@angular/core';
import { Logger } from 'angular2-logger/core';
import 'rxjs/add/operator/map';

@Injectable()
export class SignallingService {

  showDebugTabPromise: Promise<void>;
  private showDebugTabResolve;

  constructor(private logger: Logger) {
    this.showDebugTabPromise = new Promise<void>((resolve) => { this.showDebugTabResolve = resolve });
  }

  showDebugTab() {
    this.showDebugTabResolve();
  }
}
