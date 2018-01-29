import { FilterListPipe } from './filter-list.pipe';

describe('ListFilterPipe', () => {
  it('create an instance', () => {
    const pipe = new FilterListPipe();
    expect(pipe).toBeTruthy();
  });

  it('filter string array', () => {
    const pipe = new FilterListPipe();
    expect(pipe.transform(["a", "abc", "banana", "truck", "Wit"], "a")).toEqual(["a", "abc", "banana"])
  });
});
