using System;
using System.Collections.Generic;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public partial class Grid
    {
        public IEnumerable<Point> Points
        {
            get {
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        yield return new Point(x, y);
                    }
                }
            }
        }

        private IEnumerable<Point> GetColumnPoints(int column)
        {
            for (int y = 0; y < Size; y++)
            {
                yield return new Point(column, y);
            }
        }

        private IEnumerable<Point> GetRowPoints(int row)
        {
            for (int x = 0; x < Size; x++)
            {
                yield return new Point(x, row);
            }
        }

        private char GetChar(int x, int y) => grid[y, x];
        private char GetChar(Point point) => grid[point.Item2, point.Item1];

        private void SetChar(char value, int X, int Y) {
            _gridT = null;
            grid[Y, X] = value;
        }

        private void SetChar(char value, Point point) {
            _gridT = null;
            grid[point.Item2, point.Item1] = value;
        }
    }
}