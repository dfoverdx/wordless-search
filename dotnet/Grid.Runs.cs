using System;
using System.Collections.Generic;
using System.Linq;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class Grid
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
                            Word = new String(GetChar(point), length)
                        };
                    }
                }
            }
        }

        private bool FixRuns()
        {
            bool madeChange = false;

            while (FixTooManyColumnRuns() | FixTooManyRowRuns() | FixGridRuns())
            {
                madeChange = true;
            }

            return madeChange;
        }

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
                    BreakWord(run);
                }

                SetStaticWords();
            } while (madeChangeLocal);

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
                    WordPos[] runs = GetColumnRuns(x).ToArray().Shuffle();
                    if (runs.Length > Constants.MaxRunsPerRowOrColumn)
                    {
                        madeChange = true;
                        madeChangeLocal = true;
                        foreach (WordPos run in runs.Take(runs.Length - Constants.MaxRunsPerRowOrColumn))
                        {
                            BreakWord(run);
                        }
                    }
                }

                SetStaticWords();
            } while (madeChangeLocal);

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
                    WordPos[] runs = GetRowRuns(y).ToArray().Shuffle();
                    if (runs.Length > Constants.MaxRunsPerRowOrColumn)
                    {
                        madeChange = true;
                        madeChangeLocal = true;
                        foreach (WordPos run in runs.Take(runs.Length - Constants.MaxRunsPerRowOrColumn))
                        {
                            BreakWord(run);
                        }
                    }
                }

                SetStaticWords();
            } while (madeChangeLocal);

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
                        Word = new String(GetChar(point), runLen)
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
                        Word = new String(GetChar(point), runLen)
                    };
                }

                x += runLen;
            }
        }
    }
}