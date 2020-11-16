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

        private char[,] _gridT;
        private char[,] gridT
        {
            get {
                if (_gridT == null)
                {
                    _gridT = new char[Size, Size];
                    for (int y = 0; y < Size; y++)
                    {
                        for (int x = 0; x < Size; x++)
                        {
                            _gridT[x, y] = grid[y, x];
                        }
                    }
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
            _gridT = null;
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

        private string Search(Point point, Direction direction, bool includeFinal = false)
        {
            StringBuilder str = new StringBuilder(Size);
            var (x, y) = point;
            char[] block;
            int blockLen;

            switch (direction)
            {
                case Direction.North:
                    block = new char[y];
                    if (block.Length == 0)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(gridT, x * Size * sizeof(char), block, 0, y * sizeof(char));
                    str.Append(block.Reverse());
                    break;

                case Direction.South:
                    block = new char[Size - y];
                    if (block.Length == 0)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(gridT, (x * Size + y) * sizeof(char), block, 0, (Size - y) * sizeof(char));
                    str.Append(block);
                    break;

                case Direction.East:
                    block = new char[Size - x];
                    if (block.Length == 0)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(grid, (y * Size + x) * sizeof(char), block, 0, (Size - x) * sizeof(char));
                    str.Append(block);
                    break;

                case Direction.West:
                    block = new char[x];
                    if (block.Length == 0)
                    {
                        return null;
                    }

                    Buffer.BlockCopy(grid, (y * Size) * sizeof(char), block, 0, x * sizeof(char));
                    str.Append(block.Reverse());
                    break;

                case Direction.Northeast:
                    blockLen = Math.Min(y, Size - x);
                    if (blockLen == 0)
                    {
                        return null;
                    }

                    for (int i = 0; i < blockLen; i++)
                    {
                        str.Append(grid[y - i, x + i]);
                    }

                    break;

                case Direction.Northwest:
                    blockLen = Math.Min(y, x);
                    if (blockLen == 0)
                    {
                        return null;
                    }

                    for (int i = 0; i < blockLen; i++)
                    {
                        str.Append(grid[y - i, x - i]);
                    }

                    break;

                case Direction.Southeast:
                    blockLen = Math.Min(Size - y, Size - x);
                    if (blockLen == 0)
                    {
                        return null;
                    }

                    for (int i = 0; i < blockLen; i++)
                    {
                        str.Append(grid[y + i, x + i]);
                    }

                    break;

                case Direction.Southwest:
                    blockLen = Math.Min(Size - y, x);
                    if (blockLen == 0)
                    {
                        return null;
                    }

                    for (int i = 0; i < blockLen; i++)
                    {
                        str.Append(grid[y + i, x - i]);
                    }

                    break;
            }

            return Trie.findWord(str.ToString());
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