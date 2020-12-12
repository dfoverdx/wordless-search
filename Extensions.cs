using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    using System.Linq;

    public static class ArrayExtensions
    {
        public static T[][] Split<T>(this T[,] array)
        {
            int height = array.GetLength(0);
            int width = array.GetLength(1);
            int sizeOfT = typeof(T).SizeOf();
            T[][] splitArr = new T[height][];

            for (int h = 0; h < height; h++)
            {
                splitArr[h] = new T[width];
                Buffer.BlockCopy(array, h * width * sizeOfT, splitArr[h], 0, width * sizeOfT);
            }

            return splitArr;
        }
    }

    public static class IEnumerableExtensions
    {
#if DEBUG
        private static Random Random = new Random(2);
#else
        private static Random Random = new Random();
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Shuffle<T>(this IEnumerable<T> items)
        {
            T[] array = items as T[] ?? items.ToArray();
            T tmp;

            for (int i = 0; i < array.Length - 1; i++)
            {
                int idx = Random.Next(i, array.Length);
                tmp = array[idx];
                array[idx] = array[i];
                array[i] = tmp;
            }

            return array;
        }
    }
}

namespace System
{
    using System.Collections.Generic;
    using Point = Tuple<int, int>;
    // using Constants = WordlessSearch.Constants;
    using Direction = WordlessSearch.Direction;
    using Words = WordlessSearch.Words;

    public static class TypeExtensions
    {
        private static Dictionary<Type, int> typeSizes = new Dictionary<Type, int>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf(this Type type)
        {
            if (!typeSizes.ContainsKey(type))
            {
                var dynamicMethod = new DynamicMethod("SizeOf", typeof(int), Type.EmptyTypes);
                var generator = dynamicMethod.GetILGenerator();

                generator.Emit(OpCodes.Sizeof, type);
                generator.Emit(OpCodes.Ret);

                var function = (Func<int>) dynamicMethod.CreateDelegate(typeof(Func<int>));
                typeSizes.Add(type, function());
            }

            return typeSizes[type];
        }
    }

    public static class PointExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Move(this Point point, Direction direction, int distance = 1)
        {
            var (x, y) = point;

            switch (direction)
            {
                case Direction.North:
                case Direction.Northeast:
                case Direction.Northwest:
                    y -= distance;
                    break;

                case Direction.South:
                case Direction.Southeast:
                case Direction.Southwest:
                    y += distance;
                    break;
            }

            switch (direction)
            {
                case Direction.East:
                case Direction.Northeast:
                case Direction.Southeast:
                    x += distance;
                    break;

                case Direction.West:
                case Direction.Northwest:
                case Direction.Southwest:
                    x -= distance;
                    break;
            }

            return new Point(x, y);
        }

        // public static bool IsValid(this Point point) => point.Item1 >= 0 && point.Item1 < Constants.GridSize && point.Item2 >= 0 && point.Item2 < Constants.GridSize;
    }

    public static class CharExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVowel(this char c) => Words.VowelsSet.Contains(c);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConsonant(this char c) => !Words.VowelsSet.Contains(c);
    }
}