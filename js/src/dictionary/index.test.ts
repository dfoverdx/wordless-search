import findWord from './index';

describe('findWord', () => {
  it('returns null for strings shorter than 3', () => {
    expect(findWord('IS')).toBeNull();
  });

  it('returns null for strings that do not start with a word', () => {
    expect(findWord('AQQAFISHCATDOG')).toBeNull();
  });

  it('returns the word if the string is a word', () => {
    expect(findWord('AARDVARK')).toBe('AARDVARK');
  });

  it('returns the first substring of a string that starts with a word', () => {
    expect(findWord('BUTTERFLY')).toBe('BUT');
  });

  it('only returns 3+ character substrings', () => {
    expect(findWord('ISABELLA')).not.toBe('I');
    expect(findWord('ISABELLA')).not.toBe('IS');
  });
});