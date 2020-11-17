using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class Grid
    {
        private char[,] grid;

        private bool gridDirty = true;
        private readonly char[,] _gridT;
        private char[,] gridT
        {
            get {
                if (gridDirty)
                {
                    for (int y = 0; y < Size; y++)
                    {
                        for (int x = 0; x < Size; x++)
                        {
                            _gridT[x, y] = grid[y, x];
                        }
                    }

                    gridDirty = false;
                }

                return _gridT;
            }
        }

        private readonly int Size;

        private readonly WordPos[] StaticWords;

        private readonly HashSet<WordPos> StaticWordFragments;

        private readonly static Random Random = new Random();

        public bool Verbose = true;

        public Grid(int size = Constants.GridSize)
        {
            Size = size;
            grid = new char[size, size];
            _gridT = new char[size, size];
            charBuffer = new char[Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    grid[y, x] = ' ';
                }
            }

            StaticWords = new WordPos[] {
                new WordPos { Word = "FRIENDS", Direction = Direction.East, Point = new Point(1, 1) },
                new WordPos { Word = "MONEY", Direction = Direction.South, Point = new Point(size - 1, 2) }
            };

            SetStaticWords();

            StaticWordFragments = new HashSet<WordPos>();
            StaticWordFragments = FindWords().ToHashSet();

            Reset();
            SetStaticWords();
        }

        public void Reset()
        {
            gridDirty = true;
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    grid[y, x] = Words.RandomLetter();
                }
            }
        }

        private void SetStaticWords()
        {
            foreach (WordPos word in StaticWords)
            {
                SetWord(word);
            }
        }

        public void DoEvil()
        {
            if (Verbose)
            {
                Console.Clear();
            }

            while (FixWords() | FixVowels() | FixRuns());
        }
    }

    public static class PointExtensions
    {
        public static Point Move(this Point point, Direction direction, int distance = 1)
        {
            var (x, y) = point;

            switch (direction)
            {
                case Direction.North:
                case Direction.Northeast:
                case Direction.Northwest:
                    y -= distance;
                    break;

                case Direction.South:
                case Direction.Southeast:
                case Direction.Southwest:
                    y += distance;
                    break;
            }

            switch (direction)
            {
                case Direction.East:
                case Direction.Northeast:
                case Direction.Southeast:
                    x += distance;
                    break;

                case Direction.West:
                case Direction.Northwest:
                case Direction.Southwest:
                    x -= distance;
                    break;
            }

            return new Point(x, y);
        }
    }

    public enum Direction
    {
        North,
        Northwest,
        West,
        Southwest,
        South,
        Southeast,
        East,
        Northeast
    }

    public struct WordPos
    {
        public string Word;
        public int Length => Word.Length;
        public Direction Direction;
        public Point Point;

        public static bool operator==(WordPos left, WordPos right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(WordPos left, WordPos right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object other)
        {
            if (!(other is WordPos))
            {
                return false;
            }

            WordPos _other = (WordPos)other;
            return Direction == _other.Direction &&
                Word == _other.Word &&
                Point.Item1 == _other.Point.Item1 &&
                Point.Item2 == _other.Point.Item2;
        }

        public override int GetHashCode()
        {
            return Word.GetHashCode() * Direction.GetHashCode() * Point.GetHashCode();
        }
    }
}