using System;
using System.Collections.Generic;
using System.Linq;

namespace WordlessSearch
{
    public static class Trie
    {
        private static TrieNode root;

        static Trie()
        {
            root = new TrieNode(String.Empty, Words.WordsList);
        }

        private static string searchTrie(string word, string prefix, TrieNode node)
        {
            if (node.Terminal)
            {
                return prefix;
            }

            if (word == prefix || node.Nodes == null)
            {
                return null;
            }

            char nextLetter = word[prefix.Length];
            TrieNode nextNode = node.Nodes.GetValueOrDefault(nextLetter);
            if (!(nextNode is {}))
            {
                return null;
            }

            return searchTrie(word, prefix + nextLetter, nextNode);
        }

        public static string findWord(string haystack)
        {
            return searchTrie(haystack, String.Empty, root);
        }

        public struct TrieNode
        {
            public readonly bool Terminal;
            public readonly Dictionary<Char, TrieNode> Nodes;

            public TrieNode(string prefix, IEnumerable<string> words)
            {
                Terminal = words.Contains(prefix);

                var filtered = (
                    from word in words
                    where word.Length > prefix.Length
                    group word by word[prefix.Length] into groups
                    select groups
                ).ToDictionary(val => val.Key, val => new TrieNode(prefix + val.Key, val));

                if (filtered.Count > 0)
                {
                    Nodes = filtered;
                }
                else
                {
                    Nodes = null;
                }
            }
        }
    }
}