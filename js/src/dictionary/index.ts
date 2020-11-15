import words from './words';

interface TrieNode {
  readonly terminal: string | null;
  readonly nodes: Readonly<Record<string, TrieNode>> | null;
}

function makeTrieNode(prefix: string, words: string[]): TrieNode {
  const nextLetters: readonly string[] =
    Array.from(new Set(words.map(word => word[prefix.length]).filter(w => w?.length)));

  return {
    terminal: words.includes(prefix) ? prefix : null,
    nodes: nextLetters.length
      ? Object.fromEntries(nextLetters.map(letter => [
        letter,
        makeTrieNode(prefix + letter, words.filter(word => word.startsWith(prefix + letter)))
      ]))
      : null
  };
}

const dictionary = makeTrieNode('', words);

function searchTrie(word: string, prefix = '', node = dictionary): string | null {
  if (node.terminal) {
    return node.terminal;
  }

  if (word === prefix) {
    return null;
  }

  const nextLetter = word[prefix.length];
  const nextNode = node.nodes?.[nextLetter];
  return nextNode
    ? searchTrie(word, prefix + nextLetter, nextNode)
    : null;
}

/** Returns the first word found from the beginning of the haystack. */
export default function findWord(haystack: string) {
  return searchTrie(haystack.toUpperCase());
}