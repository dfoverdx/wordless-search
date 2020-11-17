using System;

namespace WordlessSearch
{
    using Point = Tuple<int, int>;

    public struct WordPos
    {
        public string Word;
        public int Length => Word.Length;
        public Direction Direction;
        public Point Point;

        public bool Contains(Point point)
        {
            var (x, y) = point;
            var (posX, posY) = Point;
            switch (Direction)
            {
                case Direction.East:
                    return y == posY && x >= posX && x < posX + Length;

                case Direction.South:
                    return x == posX && y >= posY && y < posY + Length;

                default:
                    throw new NotImplementedException();
            }
        }

        public static bool operator==(WordPos left, WordPos right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(WordPos left, WordPos right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object other)
        {
            if (!(other is WordPos))
            {
                return false;
            }

            WordPos _other = (WordPos)other;
            return Direction == _other.Direction &&
                Word == _other.Word &&
                Point.Item1 == _other.Point.Item1 &&
                Point.Item2 == _other.Point.Item2;
        }

        public override int GetHashCode()
        {
            return Word.GetHashCode() * Direction.GetHashCode() * Point.GetHashCode();
        }
    }
}