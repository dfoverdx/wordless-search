using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace WordlessSearch
{
  public static class Words
  {
    public static readonly SortedSet<string> WordsList;
    public static readonly string Letters;

    private static Random s_random = new Random();

    private static readonly (char, double)[] frequencies = new (char, double)[] {
      ( 'A', 0.082 ),
      ( 'B', 0.015 ),
      ( 'C', 0.028 ),
      ( 'D', 0.043 ),
      ( 'E', 0.13 ),
      ( 'F', 0.022 ),
      ( 'G', 0.02 ),
      ( 'H', 0.061 ),
      ( 'I', 0.07 ),
      ( 'J', 0.0015 ),
      ( 'K', 0.0077 ),
      ( 'L', 0.04 ),
      ( 'M', 0.024 ),
      ( 'N', 0.067 ),
      ( 'O', 0.075 ),
      ( 'P', 0.019 ),
      ( 'Q', 0.00095 ),
      ( 'R', 0.06 ),
      ( 'S', 0.063 ),
      ( 'T', 0.091 ),
      ( 'U', 0.028 ),
      ( 'V', 0.0098 ),
      ( 'W', 0.024 ),
      ( 'X', 0.0015 ),
      ( 'Y', 0.02 ),
      ( 'Z', 0.00074 ),
    };

    static Words()
    {
      string[] words = File.ReadAllLines("./resources/dictionary.txt");
      Letters = string.Join(string.Empty, frequencies.Select(val => new string(val.Item1, (int)(val.Item2 / 0.00074))));
      WordsList = new SortedSet<string>(
        words.Where(word => word.Length >= Constants.MinWordLength),
        StringComparer.InvariantCultureIgnoreCase);
    }

    public static char RandomLetter()
    {
      return Letters[s_random.Next(Letters.Length)];
    }
  }
}