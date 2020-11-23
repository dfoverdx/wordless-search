using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class WordlessSearch
    {
        private readonly char[,] grid = new char[Size, Size];

        private const int Size = Constants.GridSize;
        private const int LastIdx = Size - 1;

        private static readonly WordPos[] StaticWords = Constants.StaticWords;

        private readonly HashSet<WordPos> StaticWordFragments;
        private static readonly HashSet<Point> StaticWordPoints = new HashSet<Point>(StaticWords.SelectMany(w => w.Points));

        private static bool InStaticWord(Point point) => StaticWordPoints.Contains(point);
        private static bool InStaticWord(int x, int y) => StaticWordPoints.Contains(new Point(x, y));

#if DEBUG
        private static readonly Random Random = new Random(0);
#else
        private static readonly Random Random = new Random();
#endif

        private bool settingStaticWords = false;

        public bool DoPrint = true;
        private string Stage = null;

        public WordlessSearch()
        {
            charBuffer = new char[Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    grid[y, x] = ' ';
                }
            }

            SetStaticWords();

            StaticWordFragments = new HashSet<WordPos>();
            StaticWordFragments = FindWords().ToHashSet();

            Reset();
            SetStaticWords();
            SetTransforms();

            LetterCounts.Remove(' ');
        }

        public void Reset()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    grid[y, x] = Words.RandomLetter();
                    LetterCounts[grid[y, x]]++;
                }
            }
        }

        private void SetStaticWords()
        {
            settingStaticWords = true;

            foreach (WordPos word in StaticWords)
            {
                SetWord(word);
            }

            settingStaticWords = false;
        }

        public void DoEvil(CancellationToken finishedToken, CancellationToken taskToken)
        {
            bool madeChange = true;
            while (!(finishedToken.IsCancellationRequested || taskToken.IsCancellationRequested) && madeChange)
            {
                Stage = "Fix Restrictions";
                VerbosePrint(Verbosity.Normal | Verbosity.Extreme);

                while (FixRuns() | FixLetters() | FixVowels())
                {
                    VerbosePrint(Verbosity.Verbose);
                }

                madeChange = FixWords();
            }

            if (taskToken.IsCancellationRequested || finishedToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
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
}