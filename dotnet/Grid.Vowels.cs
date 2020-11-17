using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class Grid
    {
        private static readonly char[] Vowels = new char[] { 'A', 'E', 'I', 'O', 'U', 'Y' };
        private static readonly HashSet<char> VowelSet = new HashSet<char>(Vowels);
        private int CountVowels(IEnumerable<Point> points) =>
            points.Sum(point => VowelSet.Contains(grid[point.Item2, point.Item1]) ? 1 : 0);

        private int GridVowels { get => CountVowels(Points); }

        private int GetColumnVowels(int column) => CountVowels(GetColumnPoints(column));

        private int GetRowVowels(int row) => CountVowels(GetRowPoints(row));

        private bool FixVowels()
        {
            bool madeChange = FixRowVowels() | FixColumnVowels();

            while (GridVowels < Constants.MinVowels)
            {
                madeChange = true;
                int x, y;
                do
                {
                    (x, y) = (Random.Next(Size), Random.Next(Size));
                } while (VowelSet.Contains(GetChar(x, y)));

                SetChar(Vowels[Random.Next(Vowels.Length)], x, y);

                SetStaticWords();

                if (Verbose)
                {
                    Print();
                }
            }

            return madeChange;
        }

        private bool FixRowVowels()
        {
            bool madeChange = false;
            for (int y = 0; y < Size; y++)
            {
                while (GetRowVowels(y) < Constants.MinVowelsPerRowOrColumn)
                {
                    madeChange = true;
                    int x;
                    do
                    {
                        x = Random.Next(Size);
                    } while (Vowels.Contains(GetChar(x, y)));

                    SetChar(Vowels[Random.Next(Vowels.Length)], x, y);

                    SetStaticWords();
                }

                if (Verbose)
                {
                    Print();
                }
            }

            return madeChange;
        }

        private bool FixColumnVowels()
        {
            bool madeChange = false;
            for (int x = 0; x < Size; x++)
            {
                while (GetColumnVowels(x) < Constants.MinVowelsPerRowOrColumn)
                {
                    madeChange = true;
                    int y;
                    do
                    {
                        y = Random.Next(Size);
                    } while (Vowels.Contains(GetChar(x, y)));

                    SetChar(Vowels[Random.Next(Vowels.Length)], x, y);

                    SetStaticWords();
                }

                if (Verbose)
                {
                    Print();
                }
            }

            return madeChange;
        }
    }
}