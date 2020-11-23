using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class WordlessSearch
    {
        private int FindRun(Point point, Direction direction)
        {
            char start = GetChar(point);
            int count = 0;

            while (point.Item1 < Size && point.Item2 < Size && GetChar(point) == start)
            {
                count++;
                point = point.Move(direction);
            }

            return count;
        }

        private int FindVowelRun(Point point, Direction direction)
        {
            if (GetChar(point).IsConsonant())
            {
                return 0;
            }

            int count = 0;

            while (point.Item1 < Size && point.Item2 < Size && GetChar(point).IsVowel())
            {
                count++;
                point = point.Move(direction);
            }

            return count;
        }

        private IEnumerable<WordPos> FindRuns()
        {
            Direction[] directions = { Direction.South, Direction.East };
            foreach (Point point in Points)
            {
                foreach (Direction direction in directions)
                {
                    int length = FindRun(point, direction);
                    if (length > Constants.MaxRunLength)
                    {
                        yield return new WordPos {
                            Direction = direction,
                            Point = point,
                            Word = new string(GetChar(point), length)
                        };
                    }
                }
            }
        }

        private IEnumerable<WordPos> FindVowelRuns()
        {
            Direction[] directions = { Direction.South, Direction.East };
            foreach (Point point in Points)
            {
                foreach (Direction direction in directions)
                {
                    int length = FindVowelRun(point, direction);
                    if (length > Constants.MaxRunLength)
                    {
                        WordPos word = new WordPos {
                            Direction = direction,
                            Point = point,
                            Word = new string(GetChar(point), length)
                        };

                        if (word.Points.Any(p => !InStaticWord(p)))
                        {
                            yield return word;
                        }
                    }
                }
            }
        }

        private bool FixRuns() => LoopMadeChange(() => FixTooManyVowelRuns() | FixTooManyColumnRuns() | FixTooManyRowRuns() | FixGridRuns() | FixVowelRuns());

        private bool FixGridRuns()
        {
            bool madeChange = false;
            bool madeChangeLocal;

            do
            {
                madeChangeLocal = false;
                foreach (WordPos run in FindRuns())
                {
                    madeChange = true;
                    madeChangeLocal = true;
                    BreakWord(run, BreakRestriction.None);
                }
            } while (madeChangeLocal);

            if (madeChange)
            {
                VerbosePrint(Verbosity.Verbose);
            }

            return madeChange;
        }

        private bool FixTooManyColumnRuns()
        {
            bool madeChange = false;
            bool madeChangeLocal;

            do
            {
                madeChangeLocal = false;
                for (int x = 0; x < Size; x++)
                {
                    WordPos[] runs = GetColumnRuns(x).ToArray();
                    if (runs.Length > Constants.MaxRunsPerRowOrColumn)
                    {
                        madeChange = true;
                        madeChangeLocal = true;
                        foreach (WordPos run in runs.Shuffle().Take(runs.Length - Constants.MaxRunsPerRowOrColumn))
                        {
                            BreakWord(run, BreakRestriction.None);
                        }
                    }
                }
            } while (madeChangeLocal);

            if (madeChange)
            {
                VerbosePrint(Verbosity.Verbose);
            }

            return madeChange;
        }

        private bool FixTooManyRowRuns()
        {
            bool madeChange = false;
            bool madeChangeLocal;

            do
            {
                madeChangeLocal = false;
                for (int y = 0; y < Size; y++)
                {
                    WordPos[] runs = GetRowRuns(y).ToArray();
                    if (runs.Length > Constants.MaxRunsPerRowOrColumn)
                    {
                        madeChange = madeChangeLocal = true;
                        foreach (WordPos run in runs.Shuffle().Take(runs.Length - Constants.MaxRunsPerRowOrColumn))
                        {
                            BreakWord(run, BreakRestriction.None);
                        }
                    }
                }
            } while (madeChangeLocal);

            if (madeChange)
            {
                VerbosePrint(Verbosity.Verbose);
            }

            return madeChange;
        }

        private bool FixVowelRuns()
        {
            bool madeChange = false;

            IEnumerable<WordPos> vowelRuns;
            while ((vowelRuns = FindVowelRuns()).Any())
            {
                foreach (WordPos run in vowelRuns)
                {
                    madeChange = true;
                    Point point = run.Points.Where(p => !InStaticWord(p)).Shuffle().First();
                    BreakPoint(point, BreakRestriction.Consonant);
                }
            }

            VerbosePrint(Verbosity.Verbose);

            return madeChange;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool FixTooManyVowelRuns() => FixTooManyVowelRowRuns() | FixTooManyVowelColumnRuns();

        private bool FixTooManyVowelRowRuns()
        {
            bool madeChange = false;
            bool madeChangeLocal;

            do
            {
                madeChangeLocal = false;
                for (int y = 0; y < Size; y++)
                {
                    WordPos[] runs = GetRowRuns(y).Where(pos => GetChar(pos.Point).IsVowel()).ToArray();

                    if (runs.Length > Constants.MaxVowelRunsPerRowOrColumn)
                    {
                        madeChange = madeChangeLocal = true;
                        foreach (WordPos run in runs.Shuffle().Take(runs.Length - Constants.MaxVowelRunsPerRowOrColumn))
                        {
                            BreakWord(run, BreakRestriction.None);
                        }
                    }
                }
            } while (madeChangeLocal);

            if (madeChange)
            {
                VerbosePrint(Verbosity.Verbose);
            }

            return madeChange;
        }

        private bool FixTooManyVowelColumnRuns()
        {
            bool madeChange = false;
            bool madeChangeLocal;

            do
            {
                madeChangeLocal = false;
                for (int x = 0; x < Size; x++)
                {
                    WordPos[] runs = GetColumnRuns(x).Where(pos => GetChar(pos.Point).IsVowel()).ToArray();

                    if (runs.Length > Constants.MaxVowelRunsPerRowOrColumn)
                    {
                        madeChange = madeChangeLocal = true;
                        foreach (WordPos run in runs.Shuffle().Take(runs.Length - Constants.MaxVowelRunsPerRowOrColumn))
                        {
                            BreakWord(run, BreakRestriction.None);
                        }
                    }
                }
            } while (madeChangeLocal);

            if (madeChange)
            {
                VerbosePrint(Verbosity.Verbose);
            }

            return madeChange;
        }

        private IEnumerable<WordPos> GetColumnRuns(int x)
        {
            int y = 0;
            while (y < Size)
            {
                Point point = new Point(x, y);
                int runLen = FindRun(point, Direction.South);
                if (runLen > 1)
                {
                    yield return new WordPos {
                        Direction = Direction.South,
                        Point = point,
                        Word = new string(GetChar(point), runLen)
                    };
                }

                y += runLen;
            }
        }

        private IEnumerable<WordPos> GetRowRuns(int y)
        {
            int x = 0;
            while (x < Size)
            {
                Point point = new Point(x, y);
                int runLen = FindRun(point, Direction.East);
                if (runLen > 1)
                {
                    yield return new WordPos {
                        Direction = Direction.East,
                        Point = point,
                        Word = new string(GetChar(point), runLen)
                    };
                }

                x += runLen;
            }
        }
    }
}