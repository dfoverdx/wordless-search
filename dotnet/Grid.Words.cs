using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class Grid
    {
        private readonly char[] charBuffer;

        private bool FixWords()
        {
            bool madeChange = false;

            WordPos[] words = ShuffledWords;
            while (words.Length > 0)
            {
                madeChange = true;

                if (Verbose)
                {
                    Print(words.Concat(StaticWords));
                }

                foreach (WordPos word in words)
                {
                    // Don't break words that have already been broken
                    if (Search(word.Point, word.Direction) != null)
                    {
                        BreakWord(word);
                    }
                }

                SetStaticWords();

                words = ShuffledWords;

                if (Verbose)
                {
                    Print(words.Concat(StaticWords));
                }
            }

            return madeChange;
        }

        private bool FixRepeatCharacters()
        {
            return FixRepeatColumnCharacters() | FixRepeatRowCharacters();
        }

        private bool FixRepeatColumnCharacters()
        {
            bool madeChange = false;
            for (int x = 0; x < Size; x++)
            {
                Dictionary<char, List<Point>> charPoints = new Dictionary<char, List<Point>>();

                foreach (Point point in GetColumnPoints(x))
                {
                    char c = GetChar(point);
                    if (!charPoints.ContainsKey(c))
                    {
                        charPoints.Add(c, new List<Point>());
                    }

                    charPoints[c].Add(point);
                }

                foreach (var (c, points) in charPoints.Where(kvp => kvp.Value.Count > Constants.MaxSameCharacterPerRowOrColumn))
                {
                    madeChange = true;

                    Point[] ps = points.ToArray().Shuffle();
                    foreach (Point point in ps.Take(Constants.MaxSameCharacterPerRowOrColumn - ps.Length))
                    {
                        char newChar;
                        do
                        {
                            newChar = Words.RandomLetter();
                        } while (newChar == c);

                        SetChar(c, point);
                    }
                }
            }

            return madeChange;
        }

        private bool FixRepeatRowCharacters()
        {
            bool madeChange = false;
            for (int y = 0; y < Size; y++)
            {
                Dictionary<char, List<Point>> charPoints = new Dictionary<char, List<Point>>();

                foreach (Point point in GetRowPoints(y))
                {
                    char c = GetChar(point);
                    if (!charPoints.ContainsKey(c))
                    {
                        charPoints.Add(c, new List<Point>());
                    }

                    charPoints[c].Add(point);
                }

                foreach (var (c, points) in charPoints.Where(kvp => kvp.Value.Count > Constants.MaxSameCharacterPerRowOrColumn))
                {
                    madeChange = true;

                    Point[] ps = points.ToArray().Shuffle();
                    foreach (Point point in ps.Take(Constants.MaxSameCharacterPerRowOrColumn - ps.Length))
                    {
                        char newChar;
                        do
                        {
                            newChar = Words.RandomLetter();
                        } while (newChar == c);

                        SetChar(c, point);
                    }
                }
            }

            return madeChange;
        }

        public WordPos[] ShuffledWords
        {
            get => FindWords()
                .ToArray()
                .Shuffle();
        }

        private string Search(Point point, Direction direction, bool includeFinal = false)
        {
            const int CharSize = sizeof(char);
            var (x, y) = point;
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

                    for (int i = 0; i < blockLen; i++)
                    {
                        charBuffer[i] = grid[y - i, x + i];
                    }

                    str = new string(charBuffer.Take(blockLen).ToArray());
                    break;

                case Direction.Northwest:
                    blockLen = Math.Min(y, x);
                    if (blockLen < Constants.MinWordLength)
                    {
                        return null;
                    }

                    for (int i = 0; i < blockLen; i++)
                    {
                        charBuffer[i] = grid[y - i, x - i];
                    }

                    str = new string(charBuffer.Take(blockLen).ToArray());
                    break;

                case Direction.Southeast:
                    blockLen = Math.Min(Size - y, Size - x);
                    if (blockLen < Constants.MinWordLength)
                    {
                        return null;
                    }

                    for (int i = 0; i < blockLen; i++)
                    {
                        charBuffer[i] = grid[y + i, x + i];
                    }

                    str = new string(charBuffer.Take(blockLen).ToArray());
                    break;

                case Direction.Southwest:
                    blockLen = Math.Min(Size - y, x);
                    if (blockLen < Constants.MinWordLength)
                    {
                        return null;
                    }

                    for (int i = 0; i < blockLen; i++)
                    {
                        charBuffer[i] = grid[y + i, x - i];
                    }

                    str = new string(charBuffer.Take(blockLen).ToArray());
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

        private void BreakWord(WordPos word)
        {
            var (Word, length, direction, point) = (
                word.Word,
                word.Length,
                word.Direction,
                word.Point
            );

            Point target = point.Move(direction, Random.Next(length));
            SetChar(Words.RandomLetter(), target);
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
    }
}
