using System;
using System.IO;
using System.Collections.Generic;

namespace WordlessSearch
{
  public static class Words
  {
    public static readonly SortedSet<string> WordsList;
    public static readonly string Letters;

    private static Random s_random = new Random();

    static Words()
    {
      string[] words = File.ReadAllLines("./resources/dictionary.txt");
      Letters = String.Join("", words);
      WordsList = new SortedSet<string>(words, StringComparer.InvariantCultureIgnoreCase);
    }

    static char RandomLetter()
    {
      return Letters[s_random.Next(Letters.Length)];
    }
  }
}