import Direction from './Direction';

export default interface SearchResult {
  x: number;
  y: number;
  word: string;
  length: number;
  direction: Direction;
}
