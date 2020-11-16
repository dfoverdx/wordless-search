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

            Point pos = new Point(Console.CursorLeft, Console.CursorTop);

            Print(resetPos);

            Point endPos = new Point(Console.CursorLeft, Console.CursorTop + 2);

            PrintWords(words, pos);

            Console.SetCursorPosition(endPos.Item1, endPos.Item2);
        }

        public void PrintWords(IEnumerable<WordPos> words)
        {
            PrintWords(words, new Point(0, 0));
        }

        public void PrintWords(IEnumerable<WordPos> words, Point pos)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            var (left, top) = pos;

            int count = 0;
            foreach (WordPos word in words) {
                count++;

                var (x, y) = word.Point;

                for (int i = 0; i < word.Length; i++)
                {
                    Console.SetCursorPosition(left + x * 2, top + y);
                    Console.Write(GetChar(x, y));

                    (x, y) = new Point(x, y).Move(word.Direction);
                }
            }

            Console.ResetColor();

            Console.SetCursorPosition(left, top + Size + 1);
            Console.Write($"Words: {String.Format("{0,3:###}", count)}");
        }
    }
}