using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WordlessSearch
{
    public static class Words
    {
        public static readonly List<string> WordsList;
        public static readonly string Letters;
        public static readonly string Consonants;
        public static readonly string Vowels;
        public static readonly HashSet<char> VowelsSet = new HashSet<char> { 'A', 'E', 'I', 'O', 'U' };

#if DEBUG
        private static Random Random = new Random(1);
#else
        private static Random Random = new Random();
#endif

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

        public static readonly Dictionary<char, int> MinLetterCounts;

        static Words()
        {
            string[] words = File.ReadAllLines("./resources/dictionary.txt");
            Letters = string.Join(string.Empty, frequencies.Select(val => new string(val.Item1, (int)(val.Item2 * 4 / (0.00074 * 4)))));

            Consonants = string.Join(string.Empty, Letters.Where(letter => !VowelsSet.Contains(letter)));
            Vowels = string.Join(string.Empty, Letters.Where(letter => VowelsSet.Contains(letter)));

            WordsList = words.Where(word => word.Length >= Constants.MinWordLength).ToList();

            MinLetterCounts = frequencies
                .Select(val => (val.Item1, (int)(Constants.GridSize * Constants.GridSize * val.Item2 * Constants.MinDistributionRatio)))
                .ToDictionary(val => val.Item1, val => val.Item2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char RandomLetter() => Letters[Random.Next(Letters.Length)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char RandomVowel() => Vowels[Random.Next(Vowels.Length)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char RandomConsonant() => Consonants[Random.Next(Consonants.Length)];
    }
}