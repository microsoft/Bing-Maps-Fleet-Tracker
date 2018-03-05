// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { browser, element, by } from 'protractor';

export class TrackablePage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('app-root h1')).getText();
  }
}
