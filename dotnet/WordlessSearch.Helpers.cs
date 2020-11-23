using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class WordlessSearch
    {
        public IEnumerable<Point> Points
        {
            get
            {
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        yield return new Point(x, y);
                    }
                }
            }
        }

        private IEnumerable<Point> GetColumnPoints(int column)
        {
            for (int y = 0; y < Size; y++)
            {
                yield return new Point(column, y);
            }
        }

        private IEnumerable<Point> GetRowPoints(int row)
        {
            for (int x = 0; x < Size; x++)
            {
                yield return new Point(x, row);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char GetChar(int x, int y) => grid[y, x];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char GetChar(Point point) => grid[point.Item2, point.Item1];

        private char ChangeCharacter(int x, int y, BreakRestriction restriction = BreakRestriction.AttemptPreserveVowel)
        {
            char oldChar = grid[y, x];
            char newChar;
            do
            {
                switch (restriction)
                {
                    case BreakRestriction.AttemptPreserveVowel:
                        if (oldChar.IsVowel())
                        {
                            newChar = Words.RandomVowel();
                        }
                        else
                        {
                            newChar = Words.RandomLetter();
                        }

                        break;

                    case BreakRestriction.Vowel:
                        newChar = Words.RandomVowel();
                        break;

                    case BreakRestriction.Consonant:
                        newChar = Words.RandomConsonant();
                        break;

                    case BreakRestriction.None:
                    default:
                        newChar = Words.RandomLetter();
                        break;
                }
            } while (newChar == oldChar);

            return SetChar(newChar, x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char ChangeCharacter(Point point, BreakRestriction restriction = BreakRestriction.AttemptPreserveVowel) =>
            ChangeCharacter(point.Item1, point.Item2, restriction);

        private void ValidateSetChar(int x, int y)
        {
            if (Constants.VerifyOnChange && !settingStaticWords && InStaticWord(x, y))
            {
                string word = StaticWords.First(w => w.Contains(x, y)).Word;
                throw new InvalidOperationException($"Attempting to set {x},{y} which is within {word}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char SetChar(char value, int x, int y)
        {
            LetterCounts[grid[y, x]]--;
            LetterCounts[value]++;
            return grid[y, x] = gridT[x, y] = seGrid[SEDiagonalGridIdxs[y, x]] = swGrid[SWDiagonalGridIdxs[y, x]] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetChar(char value, Point point) => SetChar(value, point.Item1, point.Item2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool LoopMadeChange(Func<bool> fn)
        {
            bool madeChange = false;

            while (fn())
            {
                madeChange = true;
            }

            return madeChange;
        }
    }
}