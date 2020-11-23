using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class WordlessSearch
    {
        public override string ToString() => string.Join("\n", grid.Split().Select(row => string.Join(' ', row)));

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

            if (left + Size * 2 >= Console.WindowWidth || top + Size + 2 >= Console.WindowHeight)
            {
                Console.WriteLine(ToString());
                return;
            }

            HashSet<Point> wordPoints = CalculateWordPoints(words);

            foreach (Point point in Points)
            {
                var (x, y) = point;

                if (Constants.HighlightVowels && GetChar(point).IsVowel())
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (wordPoints.Contains(point))
                {
                    if (InStaticWord(point))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
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
            Console.WriteLine($"Words: {string.Format("{0,3:###}", words.Count())}");

            if (Stage != null)
            {
                Console.WriteLine($"\nStage: {Stage.PadRight(20)}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IfPrint(bool resetPos = true) => IfPrint(FindWords().Concat(StaticWords), resetPos);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IfPrint(IEnumerable<WordPos> words, bool resetPos = true)
        {
            if (DoPrint)
            {
                Print(words, resetPos);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void VerbosePrint(Verbosity level, bool resetPos = true) => VerbosePrint(level, FindWords().Concat(StaticWords), resetPos);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void VerbosePrint(Verbosity level, IEnumerable<WordPos> words, bool resetPos = true)
        {
            if (level.HasFlag(Constants.VerbosityLevel))
            {
                IfPrint(words, resetPos);
            }
        }

        private HashSet<Point> CalculateWordPoints(IEnumerable<WordPos> words) =>
            new HashSet<Point>(words.SelectMany(w => w.Points));
    }
}