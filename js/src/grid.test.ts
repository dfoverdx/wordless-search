import { Size } from './constants';
import Direction from './Direction';
import Grid from './grid';

const GridConstructor = Grid.constructor as new (size: number) => typeof Grid;

describe('Grid', () => {
  let grid: typeof Grid;

  beforeEach(() => {
    Grid.verbose = false;
    Grid.highlightWords = false;

    const testVal = `
      CATS
      ERAY
      ETAG
      FYAP
    `.replace(/ +/g, '').trim().split('\n').map(row => row.split(''));
    grid = new GridConstructor(4);
    grid[0] = testVal[0];
    grid[1] = testVal[1];
    grid[2] = testVal[2];
    grid[3] = testVal[3];
  });

  afterEach(jest.resetAllMocks);

  it(`is ${Size}×${Size} in length`, () => {
    expect(Grid.length).toBe(Size);
    expect(Grid.every(row => row.length === Size)).toBe(true);
  });

  it('is filled with single-character strings', () => {
    expect(Grid.every(row => row.every(cell => cell.length === 1))).toBe(true);
  });

  it('is filled with capital letters', () => {
    const str = Grid.map(row => row.join('')).join('');
    expect(str).toBe(str.toUpperCase());
  });

  describe('toString()', () => {
    it('spaces characters properly', () => {
      expect(Grid.toString()).toMatch(new RegExp(`^(([A-Z] ){${Size - 1}}[A-Z]\n\n){${Size - 1}}([A-Z] ){${Size - 1}}[A-Z]$`));
    });

    it('colors things properly', () => {
      grid.highlightWords = true;

    });
  });

  describe('findWords()', () => {
    it('finds the correct words', () => {
      expect(grid.findWords()).toMatchSnapshot();
    });
  });

  describe('breakWord()', () => {
    it('replaces a random letter within the word a different letter', () => {
      const spy = jest.spyOn(Math, 'randomInt');
      spy.mockReturnValueOnce(1);
      spy.mockReturnValueOnce(65);
      spy.mockReturnValueOnce(66);

      grid.breakWord({
        x: 0,
        y: 0,
        direction: Direction.East,
        length: 3,
        word: 'CAT',
      });

      expect(grid[0]).toEqual('CBTS'.split(''));
    });
  });
});