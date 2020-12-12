using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class WordlessSearch
    {
        private readonly Dictionary<char, int> LetterCounts = Enumerable.Range((int)'A', (int)'Z')
            .Append((int)' ')
            .ToDictionary(val => (char)val, _ => 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool FixLetters() => LoopMadeChange(() => FixRepeatColumnLetters() | FixRepeatRowLetters() | FixLetterDistribution());

        private bool FixRepeatColumnLetters()
        {
            bool madeChange = false;
            for (int x = 0; x < Size; x++)
            {
                Dictionary<char, List<Point>> charPoints = new Dictionary<char, List<Point>>();

                foreach (Point point in GetColumnPoints(x))
                {
                    char c = GetChar(point);
                    if (!charPoints.ContainsKey(c))
                    {
                        charPoints.Add(c, new List<Point>());
                    }

                    charPoints[c].Add(point);
                }

                foreach (var (c, points) in charPoints.Where(kvp => kvp.Value.Count > Constants.MaxSameCharacterPerRowOrColumn))
                {
                    madeChange = true;

                    IEnumerable<Point> ps = points.ToArray().Shuffle();
                    ps = ps
                        .Where(p => !InStaticWord(p))
                        .Take(ps.Count() - Constants.MaxSameCharacterPerRowOrColumn);

                    foreach (Point point in ps)
                    {
                        BreakPoint(point);
                    }
                }
            }

            VerbosePrint(Verbosity.Verbose);
            return madeChange;
        }

        private bool FixRepeatRowLetters()
        {
            bool madeChange = false;
            for (int y = 0; y < Size; y++)
            {
                Dictionary<char, List<Point>> charPoints = new Dictionary<char, List<Point>>();

                foreach (Point point in GetRowPoints(y))
                {
                    char c = GetChar(point);
                    if (!charPoints.ContainsKey(c))
                    {
                        charPoints.Add(c, new List<Point>());
                    }

                    charPoints[c].Add(point);
                }

                var kvps = charPoints.Where(kvp => kvp.Value.Count > Constants.MaxSameCharacterPerRowOrColumn);
                foreach (var (c, points) in kvps)
                {
                    madeChange = true;

                    IEnumerable<Point> ps = points.ToArray().Shuffle();
                    ps = ps
                        .Where(p => !InStaticWord(p))
                        .Take(ps.Count() - Constants.MaxSameCharacterPerRowOrColumn);

                    foreach (Point point in ps)
                    {
                        BreakPoint(point);
                    }
                }
            }

            VerbosePrint(Verbosity.Verbose);
            return madeChange;
        }

        private bool FixLetterDistribution() => LoopMadeChange(() =>
        {
            const int MaxPointsToTry = 20;
            bool madeChange = false;
            HashSet<char> lettersFixed = new HashSet<char>();

            foreach (var (c, min) in Words.MinLetterCounts.Shuffle())
            {
                if (LetterCounts[c] >= min)
                {
                    continue;
                }

                madeChange = true;
                int lettersChanged = 0;
                int lettersToChange = min - LetterCounts[c];

                lettersFixed.Add(c);
                Point[] otherCharPoints = Points.Where(p => !lettersFixed.Contains(GetChar(p)) && !InStaticWord(p)).Shuffle();
                int pointIdx = 0;
                int pointsTried = 0;

                while (lettersChanged < lettersToChange && pointIdx < otherCharPoints.Length)
                {
                    Point point = otherCharPoints[pointIdx++];
                    bool wasPartOfWord = PointContainsWord(point);
                    char prevChar = GetChar(point);
                    SetChar(c, point);
                    VerbosePrint(Verbosity.Extreme);

                    if (!wasPartOfWord && PointContainsWord(point) && pointsTried < MaxPointsToTry && pointIdx < otherCharPoints.Length)
                    {
                        SetChar(prevChar, point);
                        pointsTried++;
                    }
                    else
                    {
                        lettersChanged++;
                    }
                }
            }

            VerbosePrint(Verbosity.Verbose);
            return madeChange;
        });
    }
}
