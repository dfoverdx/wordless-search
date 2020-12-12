using System;
using System.Collections.Generic;
using System.Linq;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class WordlessSearch
    {
        public WordPos[] ShuffledWords => FindWords().Shuffle();

        private readonly char[] charBuffer = new char[Size];

        private bool FixWords()
        {
            Stage = "Remove Words";
            bool madeChange = false;

            WordPos[] words = ShuffledWords;
            VerbosePrint(Verbosity.Normal | Verbosity.Verbose, words.Concat(StaticWords));

            while (words.Length > 0)
            {
                madeChange = true;

                foreach (WordPos word in words)
                {
                    // Don't break words that have already been broken
                    if (Search(word.Point, word.Direction) != null)
                    {
                        BreakWord(word);
                    }
                }

                words = ShuffledWords;
                VerbosePrint(Verbosity.Verbose, words.Concat(StaticWords));
            }

            Stage = null;
            // VerbosePrint(Verbosity.Normal, StaticWords);
            return madeChange;
        }

        private string Search(Point point, Direction direction, bool includeFinal = false)
        {
            const int CharSize = sizeof(char);
            var (x, y) = point;
            int start;
            int diagLen;
            int blockLen;
            string str;

            switch (direction)
            {
                case Direction.North:
                    if (y < Constants.MinWordLength - 1)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(gridT, x * Size * CharSize, charBuffer, 0, Size * CharSize);
                    str = new string(charBuffer.Take(y + 1).Reverse().ToArray());
                    break;

                case Direction.South:
                    if (y > Size - Constants.MinWordLength - 1)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(gridT, x * Size * CharSize, charBuffer, 0, Size * CharSize);
                    str = new string(charBuffer.Skip(y).ToArray());
                    break;

                case Direction.East:
                    if (x > Size - Constants.MinWordLength - 1)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(grid, y * Size * CharSize, charBuffer, 0, Size * CharSize);
                    str = new string(charBuffer.Skip(x).ToArray());
                    break;

                case Direction.West:
                    if (x < Constants.MinWordLength - 1)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(grid, y * Size * CharSize, charBuffer, 0, Size * CharSize);
                    str = new string(charBuffer.Take(x + 1).Reverse().ToArray());
                    break;

                case Direction.Northeast:
                    blockLen = Math.Min(y, Size - x);
                    if (blockLen < Constants.MinWordLength)
                    {
                        return null;
                    }

                    (start, diagLen) = DiagonalLengths[SWDiagonalIdxs[y, x]];

                    Buffer.BlockCopy(swGrid, start * CharSize, charBuffer, 0, diagLen * CharSize);

                    str = new string(charBuffer.Take(blockLen).Reverse().ToArray());
                    break;

                case Direction.Northwest:
                    blockLen = Math.Min(y, x);
                    if (blockLen < Constants.MinWordLength)
                    {
                        return null;
                    }

                    (start, diagLen) = DiagonalLengths[SEDiagonalIdxs[y, x]];

                    Buffer.BlockCopy(seGrid, start * CharSize, charBuffer, 0, diagLen * CharSize);

                    str = new string(charBuffer.Take(blockLen).Reverse().ToArray());
                    break;

                case Direction.Southeast:
                    blockLen = Math.Min(Size - y, Size - x);
                    if (blockLen < Constants.MinWordLength)
                    {
                        return null;
                    }

                    (start, diagLen) = DiagonalLengths[SEDiagonalIdxs[y, x]];

                    Buffer.BlockCopy(seGrid, start * CharSize, charBuffer, 0, diagLen * CharSize);

                    str = new string(charBuffer.Skip(diagLen - blockLen).Take(blockLen).ToArray());
                    break;

                case Direction.Southwest:
                    blockLen = Math.Min(Size - y, x + 1);
                    if (blockLen < Constants.MinWordLength)
                    {
                        return null;
                    }

                    (start, diagLen) = DiagonalLengths[SWDiagonalIdxs[y, x]];

                    Buffer.BlockCopy(swGrid, start * CharSize, charBuffer, 0, diagLen * CharSize);

                    str = new string(charBuffer.Skip(diagLen - blockLen).Take(blockLen).ToArray());
                    break;

                default:
                    // Should never get here...?
                    throw new Exception("Wut?");
            }

            return Trie.findWord(str);
        }

        public IEnumerable<WordPos> FindWords()
        {
            foreach (Point p in Points)
            {
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    string word = Search(p, d);
                    if (word == null)
                    {
                        continue;
                    }

                    WordPos result = new WordPos
                    {
                        Point = p,
                        Direction = d,
                        Word = word
                    };

                    if (!StaticWordFragments.Contains(result))
                    {
                        yield return result;
                    }
                }
            }
        }

        private void BreakWord(WordPos word, BreakRestriction restriction = BreakRestriction.AttemptPreserveVowel)
        {
            var (Word, length, direction, point) = (
                word.Word,
                word.Length,
                word.Direction,
                word.Point
            );

            Point target = null;
            char prevChar = '\0';
            int tries = 0;
            IEnumerable<Point> points = word.Points.Where(p => !InStaticWord(p));
            double maxTries = Math.Pow(10, points.Count());

            do
            {
                if (tries >= 5 && restriction == BreakRestriction.AttemptPreserveVowel)
                {
                    restriction = BreakRestriction.None;
                }

                if (target != null)
                {
                    if (tries < 26)
                    {
                        SetChar(prevChar, target);
                    }
                }

                target = points.Shuffle().First();
                prevChar = GetChar(target);
                ChangeCharacter(target, restriction);
            } while (++tries < maxTries && PointContainsWord(target));

            VerbosePrint(Verbosity.Extreme);
        }

        private void BreakPoint(Point point, BreakRestriction restriction = BreakRestriction.AttemptPreserveVowel)
        {
            int tries = 0;
            int maxTries;

            switch (restriction)
            {
                case BreakRestriction.Vowel:
                    maxTries = 5;
                    break;

                case BreakRestriction.Consonant:
                    maxTries = 30;
                    break;

                case BreakRestriction.AttemptPreserveVowel:
                case BreakRestriction.None:
                default:
                    maxTries = 40;
                    break;
            }

            do
            {
                if (restriction == BreakRestriction.AttemptPreserveVowel && tries > 5)
                {
                    restriction = BreakRestriction.None;
                }

                ChangeCharacter(point, restriction);
            } while (++tries < maxTries && PointContainsWord(point));

            VerbosePrint(Verbosity.Extreme);
        }

        private bool PointContainsWord(Point point)
        {
            var (x, y) = point;

            int maxD = Math.Max(Math.Max(y, x), Math.Max(Size - y, Size - x));

            for (int d = 0; d < maxD; d++)
            {
                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Point p;
                    switch (dir)
                    {
                        case Direction.East:
                            p = point.Move(Direction.West, d);
                            break;
                        case Direction.West:
                            p = point.Move(Direction.East, d);
                            break;
                        case Direction.North:
                            p = point.Move(Direction.South, d);
                            break;
                        case Direction.South:
                            p = point.Move(Direction.North, d);
                            break;
                        case Direction.Northeast:
                            p = point.Move(Direction.Southwest, d);
                            break;
                        case Direction.Southwest:
                            p = point.Move(Direction.Northeast, d);
                            break;
                        case Direction.Northwest:
                            p = point.Move(Direction.Southeast, d);
                            break;
                        case Direction.Southeast:
                            p = point.Move(Direction.Northwest, d);
                            break;
                        default:
                            throw new Exception("Should not get here");
                    }

                    if (p.Item1 < 0 || p.Item1 > LastIdx || p.Item2 < 0 || p.Item2 > LastIdx)
                    {
                        continue;
                    }

                    string word = Search(p, dir);
                    if (word?.Length > d)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void SetWord(WordPos word)
        {
            var (x, y) = word.Point;

            SetChar(word.Word[0], word.Point);
            for (int i = 1; i < word.Word.Length; i++)
            {
                switch (word.Direction)
                {
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

                switch (word.Direction)
                {
                    case Direction.East:
                    case Direction.Northeast:
                    case Direction.Southeast:
                        x++;
                        break;

                    case Direction.West:
                    case Direction.Northwest:
                    case Direction.Southwest:
                        x++;
                        break;
                }

                SetChar(word.Word[i], x, y);
            }
        }

        private enum BreakRestriction
        {
            None,
            Consonant,
            Vowel,
            AttemptPreserveVowel,
        }
    }
}
