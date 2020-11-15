import 'colors';
import './util';
import { MinVowels, Size } from './constants';
import findWord from './dictionary';
import words from './dictionary/words';
import Direction from './Direction';
import SearchResult from './SearchResult';

const AllLetters = words.join('');
const LOVE = Array.from('FUN');
const MONEY = Array.from('GREAT');

const Directions = [
  Direction.North,
  Direction.Northwest,
  Direction.West,
  Direction.Southwest,
  Direction.South,
  Direction.Southeast,
  Direction.East,
  Direction.Northeast,
] as const;

const Grid = new class Grid extends Array<string[]> {
  private readonly Size: number;

  verbose = false;
  highlightWords = false;

  constructor(size = Size) {
    super(size);

    this.Size = size;
  }

  reset() {
    for (let i = 0; i < this.Size; i++) {
      this[i] = new Array(this.Size).fill(() => randomLetter()).map(fn => fn());
    }

    this.setConstantWords();
  }

  setConstantWords() {
    for (let i = 0; i < LOVE.length; i++) {
      this[1][i + 2] = LOVE[i];
    }

    for (let i = 0; i < MONEY.length; i++) {
      this[i + 3][this.Size - 1] = MONEY[i];
    }
  }

  findWords(includeFinal = false): SearchResult[] {
    const results: SearchResult[] = [];

    for (let x = 0; x < this.Size; x++) {
      for (let y = 0; y < this.Size; y++) {
        for (const direction of Directions) {
          const word = this.search(x, y, direction, includeFinal);
          if (word) {
            results.push({ x, y, direction, length: word.length, word });
          }
        }
      }
    }

    return results;
  }

  breakWord({ x, y, direction, word, length }: SearchResult): void {
    const idx = Math.randomInt(0, length);
    switch (direction) {
      case Direction.North:
      case Direction.Northeast:
      case Direction.Northwest:
        y -= idx;
        break;

      case Direction.South:
      case Direction.Southeast:
      case Direction.Southwest:
        y += idx;
        break;
    }

    switch (direction) {
      case Direction.East:
      case Direction.Northeast:
      case Direction.Southeast:
        x += idx;
        break;

      case Direction.West:
      case Direction.Northwest:
      case Direction.Southwest:
        x -= idx;
        break;
    }

    this[y][x] = randomLetter(word[idx]);
  }

  allowOnlyNWords(): void {
    this.reset();

    setCursorPos();

    const vowelList = ['A', 'E', 'I', 'O', 'U'];
    const vowelSet = new Set(vowelList);

    const goodWords = this.findWords(true).filter(({ word }) => word === 'MONEY' || word === 'LOVE');

    while (true) {
      let words = this.findWords().shuffle().sort(({ length: l1 }, { length: l2 }) => l1 - l2);

      if (this.verbose) {
        setCursorPos();
      }

      this.verboseLog(this.toString([...words, ...goodWords]));

      while (words.length) {
        words = words.slice(0, Math.ceil(words.length / 2));
        // } else {
        //   const numChanges = Math.ceil((words.length - numAllowedWords) / 2);
        //   words = words.slice(0, numChanges);
        // }

        // this.verboseLog(`\n\nBreaking ${words.length} words`);
        for (const brk of words) {
          this.breakWord(brk);
        }

        this.setConstantWords();
        words = this.findWords().shuffle().sort(({ length: l1 }, { length: l2 }) => l1 - l2);
        // minWordLength = words.reduce((min, { word }) => Math.min(min, word.length), Infinity);
        if (this.verbose) {
          setCursorPos();
          // console.clear();
        }

        this.verboseLog(this.toString([...words, ...goodWords]));
      }

      const vowelCount = this.flat().filter(l => vowelSet.has(l)).length;

      if (vowelCount < MinVowels) {
        for (let i = 0; i < MinVowels - vowelCount; i++) {
          let x: number;
          let y: number;
          do {
            x = Math.randomInt(this.Size);
            y = Math.randomInt(this.Size);
          } while (vowelSet.has(this[y][x]));

          this[y][x] = vowelList[Math.randomInt(vowelList.length)];
        }

        this.setConstantWords();
        continue;
      }

      break;
    };
  }

  private search(x: number, y: number, direction: Direction, includeFinal = false): string | null {
    let str = '';

    switch (direction) {
      case Direction.North:
        str = this.map(row => row[x]).reverse().slice(this.Size - y - 1).join('');
        break;

      case Direction.South:
        if (!includeFinal && x === this.Size - 1 && y >= 3 && y < 3 + MONEY.length) {
          return null;
        }

        str = this.map(row => row[x]).slice(y).join('');
        break;

      case Direction.East:
        if (!includeFinal && y === 1 && x >= 2 && x < 2 + LOVE.length) {
          return null;
        }

        str = this[y].slice(x).join('');
        break;

      case Direction.West:
        str = this[y].slice().reverse().slice(this.Size - x - 1).join('');
        break;

      case Direction.Southeast:
        for (let i = 0; x + i < this.Size && y + i < this.Size; i++) {
          str += this[y + i][x + i];
        }
        break;

      case Direction.Northeast:
        for (let i = 0; x + i < this.Size && y - i >= 0; i++) {
          str += this[y - i][x + i];
        }
        break;

      case Direction.Northwest:
        for (let i = 0; x - i >= 0 && y - i >= 0; i++) {
          str += this[y - i][x - i];
        }
        break;

      case Direction.Southwest:
        for (let i = 0; x - i >= 0 && y + i < this.Size; i++) {
          str += this[y + i][x - i];
        }
        break;
    }

    return findWord(str);
  }

  private verboseLog(...args: any) {
    if (this.verbose) {
      console.log(...args);
    }
  }

  toString(words?: SearchResult[]) {
    let copy: string[][] = this;
    if (this.highlightWords) {
      copy = this.map(row => row.slice());
      (words ?? this.findWords()).reduce(highlightWord, copy);
    }

    return copy.map(row => row.join(' ')).join('\n\n');
  }

  highlight(word: SearchResult) {
    const copy = this.map(row => row.slice());
    console.log(word.word.yellow.bold);
    console.log('');
    console.log(highlightWord(copy, word).map(row => row.join(' ')).join('\n\n'));
  }
}

export default Grid;

function randomLetter(not?: string) {
  let letter: string;

  do {
    letter = AllLetters[Math.randomInt(AllLetters.length)];
  } while (letter === not);

  return letter;
}

const highlightDirection = {
  [Direction.North](str: string) { return str.strip.blue.inverse; },
  [Direction.South](str: string) { return str.strip.blue; },
  [Direction.East](str: string) { return str.strip.red; },
  [Direction.West](str: string) { return str.strip.red.inverse; },
  [Direction.Northeast](str: string) { return str.strip.yellow; },
  [Direction.Northwest](str: string) { return str.strip.yellow.inverse; },
  [Direction.Southeast](str: string) { return str.strip.magenta; },
  [Direction.Southwest](str: string) { return str.strip.magenta.inverse; },
};

function highlightWord(grid: string[][], { x, y, direction, length }: SearchResult): string[][] {
  // const color = (str: string) => highlightDirection[direction];
  // grid[y][x] = color(grid[y][x]);
  grid[y][x] = grid[y][x].yellow;

  for (let i = 1; i < length; i++) {
    switch (direction) {
      case Direction.North:
      case Direction.Northeast:
      case Direction.Northwest:
        y--;
        break;

      case Direction.South:
      case Direction.Southeast:
      case Direction.Southwest:
        y++;
        break;
    }

    switch (direction) {
      case Direction.East:
      case Direction.Northeast:
      case Direction.Southeast:
        x++;
        break;

      case Direction.West:
      case Direction.Northwest:
      case Direction.Southwest:
        x--;
        break;
    }

    grid[y][x] = grid[y][x].yellow;
    // grid[y][x] = color(grid[y][x]);
  }

  return grid;
}

function setCursorPos(x = 0, y = 0) {
  console.log('\x1b[' + y + ';' + x + 'H');
}