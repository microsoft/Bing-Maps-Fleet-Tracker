// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TrackablePage } from './app.po';

describe('trackable App', function() {
  let page: TrackablePage;

  beforeEach(() => {
    page = new TrackablePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect<any>(page.getParagraphText()).toEqual('app works!'); 
  });
});
