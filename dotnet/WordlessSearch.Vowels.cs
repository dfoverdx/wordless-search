using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class WordlessSearch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CountVowels(IEnumerable<Point> points) => points.Count(point => GetChar(point).IsVowel());

        private int GridVowels { get => CountVowels(Points); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetColumnVowels(int column) => CountVowels(GetColumnPoints(column));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRowVowels(int row) => CountVowels(GetRowPoints(row));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool FixVowels() => false;
            // FixRowVowels() |
            // FixColumnVowels();// |
            // FixMinDistinctVowels();

        private bool FixRowVowels()
        {
            bool madeChange = false;
            for (int y = 0; y < Size; y++)
            {
                bool madeChangeLocal = false;

                int rowVowels = GetRowVowels(y);

                if (rowVowels < Constants.MinVowelsPerRowOrColumn)
                {
                    var points = GetRowPoints(y)
                        .Where(p => GetChar(p).IsConsonant() && !InStaticWord(p))
                        .Shuffle()
                        .Take(Constants.MinVowelsPerRowOrColumn - rowVowels);

                    foreach (Point point in points)
                    {
                        madeChange = madeChangeLocal = true;
                        BreakPoint(point, BreakRestriction.Vowel);
                    }
                }
                else if (rowVowels > Constants.MaxVowelsPerRowOrColumn)
                {
                    var points = GetRowPoints(y)
                        .Where(p => GetChar(p).IsVowel() && !InStaticWord(p))
                        .Shuffle()
                        .Take(rowVowels - Constants.MaxVowelsPerRowOrColumn);

                    foreach (Point point in points)
                    {
                        madeChange = madeChangeLocal = true;
                        BreakPoint(point, BreakRestriction.Consonant);
                    }
                }

                if (madeChangeLocal)
                {
                    VerbosePrint(Verbosity.Verbose);
                }
            }

            return madeChange;
        }

        private bool FixColumnVowels()
        {
            bool madeChange = false;
            for (int x = 0; x < Size; x++)
            {
                bool madeChangeLocal = false;

                int colVowels = GetColumnVowels(x);

                if (colVowels < Constants.MinVowelsPerRowOrColumn)
                {
                    var points = GetColumnPoints(x)
                        .Where(p => GetChar(p).IsConsonant() && !InStaticWord(p))
                        .Shuffle()
                        .Take(Constants.MinVowelsPerRowOrColumn - colVowels);

                    foreach (Point point in points)
                    {
                        madeChange = madeChangeLocal = true;
                        BreakPoint(point, BreakRestriction.Vowel);
                    }
                }
                else if (colVowels > Constants.MaxVowelsPerRowOrColumn)
                {
                    var points = GetColumnPoints(x)
                        .Where(p => GetChar(p).IsVowel() && !InStaticWord(p))
                        .Shuffle()
                        .Take(colVowels - Constants.MaxVowelsPerRowOrColumn);

                    foreach (Point point in points)
                    {
                        madeChange = madeChangeLocal = true;
                        BreakPoint(point, BreakRestriction.Consonant);
                    }
                }

                if (madeChangeLocal)
                {
                    VerbosePrint(Verbosity.Verbose);
                }
            }

            return madeChange;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool FixMinDistinctVowels() => FixMinDistinctRowVowels() | FixMinDistinctColumnVowels();

        private bool FixMinDistinctRowVowels()
        {
            bool madeChange = false;

            for (int y = 0; y < Size; y++)
            {
                var vowelPoints = GetRowPoints(y).Where(p => GetChar(p).IsVowel());
                int count;
                int tries = 0;
                const int maxTries = 30;

                while (++tries < maxTries && (count = vowelPoints.Distinct().Count()) < Constants.MinDistinctVowelsPerRowOrColumn)
                {
                    madeChange = true;
                    vowelPoints = vowelPoints
                        .Where(p => !InStaticWord(p))
                        .Shuffle()
                        .Take(Constants.MinDistinctVowelsPerRowOrColumn - count);

                    foreach (Point point in vowelPoints)
                    {
                        BreakPoint(point);
                    }
                }
            }

            return madeChange;
        }

        private bool FixMinDistinctColumnVowels()
        {
            bool madeChange = false;

            for (int x = 0; x < Size; x++)
            {
                var vowelPoints = GetColumnPoints(x).Where(p => GetChar(p).IsVowel());
                int count;
                int tries = 0;
                const int maxTries = 30;

                while (++tries < maxTries && (count = vowelPoints.Distinct().Count()) < Constants.MinDistinctVowelsPerRowOrColumn)
                {
                    madeChange = true;
                    vowelPoints = vowelPoints
                        .Where(p => !InStaticWord(p))
                        .Shuffle()
                        .Take(Constants.MinDistinctVowelsPerRowOrColumn - count);

                    foreach (Point point in vowelPoints)
                    {
                        BreakPoint(point);
                    }
                }
            }

            return madeChange;
        }
    }
}