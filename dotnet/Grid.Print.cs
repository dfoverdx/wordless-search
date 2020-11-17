using System;
using System.Collections.Generic;
using System.Linq;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class Grid
    {
                public override string ToString()
        {
            char[][] rows = new char[Size][];
            for (int r = 0; r < Size; r++)
            {
                rows[r] = new char[Size];
                Buffer.BlockCopy(grid, r * Size * sizeof(char), rows[r], 0, Size * sizeof(char));
            }

            return String.Join("\n", rows.Select(row => String.Join(' ', row)));
        }

        public void Print()
        {
            Print(true, true);
        }

        public void Print(bool resetPos)
        {
            if (resetPos)
            {
                Console.SetCursorPosition(0, 0);
            }

            Console.WriteLine(this.ToString());
        }

        public void Print(bool highlighted, bool resetPos)
        {
            if (!highlighted)
            {
                Print(resetPos);
            }
            else
            {
                Print(FindWords().Concat(StaticWords), resetPos);
            }
        }

        public void Print(IEnumerable<WordPos> words, bool resetPos = true)
        {
            if (resetPos)
            {
                Console.SetCursorPosition(0, 0);
            }

            var (left, top) = (Console.CursorLeft, Console.CursorTop);
            HashSet<Point> wordPoints = CalculateWordPoints(words);

            foreach (Point point in Points)
            {
                var (x, y) = point;

                if (wordPoints.Contains(point))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.SetCursorPosition(left + x * 2, top + y);
                Console.Write(GetChar(point));
            }

            Console.ResetColor();

            Console.SetCursorPosition(left, top + Size + 1);
            Console.WriteLine($"Words: {String.Format("{0,3:###}", words.Count())}");
        }

        private HashSet<Point> CalculateWordPoints(IEnumerable<WordPos> words)
        {
            HashSet<Point> points = new HashSet<Point>();
            foreach (WordPos word in words)
            {
                var (length, direction, (x, y)) = (
                    word.Length,
                    word.Direction,
                    word.Point
                );

                for (int i = 0; i < word.Length; i++)
                {
                    points.Add(new Point(x, y).Move(direction, i));
                }
            }

            return points;
        }
    }
}