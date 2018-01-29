import { TrackablePage } from './app.po';

describe('trackable App', function() {
  let page: TrackablePage;

  beforeEach(() => {
    page = new TrackablePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
