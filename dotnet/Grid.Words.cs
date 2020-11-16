using System;
using System.Collections.Generic;
using System.Linq;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class Grid
    {
        private bool FixWords()
        {
            bool madeChange = false;

            WordPos[] words;
            while ((words = ShuffledWords).Length > 0)
            {
                madeChange = true;

                if (Verbose)
                {
                    Print(words.Concat(StaticWords));
                }

                foreach (WordPos word in words.Take(words.Length / 2 + words.Length % 2))
                {
                    BreakWord(word);
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

        public WordPos[] ShuffledWords
        {
            get => FindWords()
                .ToArray()
                .Shuffle()
                .OrderBy(item => item.Length)
                .ToArray();
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
