import './util';

describe('Math.randomInt()', () => {
  it('only produces a random numbers between min and max inclusive', () => {
    const set = new Set<number>();
    for (let i = 0; i < 1000; i++) {
      set.add(Math.randomInt(1, 6));
    }

    expect(set).toEqual(new Set([ 1, 2, 3, 4, 5 ]));
  });
});