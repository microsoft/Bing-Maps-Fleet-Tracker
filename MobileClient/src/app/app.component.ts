import { Component } from '@angular/core';
import { Platform } from 'ionic-angular';
import { StatusBar, Splashscreen } from 'ionic-native';
import { Logger } from 'angular2-logger/core';

import { TabsPage } from '../pages/tabs/tabs';

declare var hockeyapp;

@Component({
  templateUrl: 'app.html'
})
export class MyApp {
  rootPage = TabsPage;

  hockeyAppAndroidAppID = '8a4b18ea9efb48f2b65c1de54fdb93b7';
  hockeyAppIosAppID = '0a65982c7c10475e9e879c5d4c259d08';

  constructor(
    private platform: Platform,
    private logger: Logger) {

    platform.ready().then(() => {
      // Okay, so the platform is ready and our plugins are available.
      // Here you can do any higher level native things you might need.
      let appID;

      if (platform.is('android')) {
        appID = this.hockeyAppAndroidAppID;
      }
      else if (platform.is('ios')) {
        appID = this.hockeyAppIosAppID;
      }

      if (appID) {
        hockeyapp.start(() => this.hockeyappSuccess(), (error) => this.hockeyappFailure(error), appID);
      }

      StatusBar.styleDefault();
      Splashscreen.hide();
    });
  }

  hockeyappSuccess() {
    this.logger.info('HockeyApp init success');
    hockeyapp.checkForUpdate(() => {
      this.logger.info('check for update success');
    }, () => {
      this.logger.error('check for update failure');
    });
  }

  hockeyappFailure(error) {
    this.logger.error('HockeyApp init failure =>' + error);
  }
}
