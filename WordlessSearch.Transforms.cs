using System;

namespace WordlessSearch
{
    public partial class WordlessSearch
    {
        private readonly char[,] gridT = new char[Size, Size];
        private readonly char[] seGrid = new char[Size * Size];
        private readonly char[] swGrid = new char[Size * Size];

        private void SetTransforms()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    gridT[y, x] = grid[x, y];
                    seGrid[SEDiagonalGridIdxs[y, x]] = grid[y, x];
                    swGrid[SWDiagonalGridIdxs[y, x]] = grid[y, x];
                }
            }
        }

        private static readonly (int, int)[] DiagonalLengths = new Func<(int, int)[]>(() => {
            const int DIAGS = 2 * Size - 1;
            const int MID_DIAG = DIAGS / 2 + DIAGS % 2;

            (int, int)[] lengths = new (int, int)[DIAGS];
            int cumLen = 0;

            for (int i = 0; i < MID_DIAG; i++)
            {
                lengths[i] = (cumLen, i + 1);
                cumLen += i + 1;
            }

            for (int i = 0; i < DIAGS / 2; i++)
            {
                lengths[MID_DIAG + i] = (cumLen, LastIdx - i);
                cumLen += LastIdx - i;
            }

            return lengths;
        })();

        /// <summary>
        /// y,x -> swGrid[i]
        /// </summary>
        private static readonly int[,] SEDiagonalGridIdxs = new Func<int[,]>(() => {
            int[,] idxs = new int[Size, Size];
            int i = 0;

            for (int y = LastIdx; y >= 0; y--)
            {
                for (int x = 0; x < Size - y; x++)
                {
                    idxs[y + x, x] = i++;
                }
            }

            for (int x = 1; x < Size; x++)
            {
                for (int y = 0; y < Size - x; y++)
                {
                    idxs[y, x + y] = i++;
                }
            }

            return idxs;
        })();

        private static readonly int[,] SEDiagonalIdxs = new Func<int[,]>(() => {
            int[,] idxs = new int[Size, Size];

            int i = 0;
            for (int y = LastIdx; y >= 0; y--, i++)
            {
                for (int x = 0; x < Size - y; x++)
                {
                    idxs[y + x, x] = i;
                }
            }

            for (int x = 1; x < Size; x++, i++)
            {
                for (int y = 0; y < Size - x; y++)
                {
                    idxs[y, x + y] = i;
                }
            }

            return idxs;
        })();

        private static readonly int[,] SWDiagonalGridIdxs = new Func<int[,]>(() => {
            int[,] idxs = new int[Size, Size];
            int i = 0;

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < x + 1; y++)
                {
                    idxs[y, x - y] = i++;
                }
            }

            for (int y = 1; y < Size; y++)
            {
                for (int x = LastIdx; x >= y; x--)
                {
                    idxs[y + LastIdx - x, x] = i++;
                }
            }

            return idxs;
        })();

        private static readonly int[,] SWDiagonalIdxs = new Func<int[,]>(() => {
            int[,] idxs = new int[Size, Size];
            int i = 0;

            for (int x = 0; x < Size; x++, i++)
            {
                for (int y = 0; y < x + 1; y++)
                {
                    idxs[y, x - y] = i;
                }
            }

            for (int y = 1; y < Size; y++, i++)
            {
                for (int x = LastIdx; x >= y; x--)
                {
                    idxs[y + LastIdx - x, x] = i;
                }
            }

            return idxs;
        })();
    }
}