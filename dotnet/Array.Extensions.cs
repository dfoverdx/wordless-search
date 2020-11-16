using System.Collections.Generic;

namespace System
{
    public static class ArrayExtensions
    {
        private static Random Random = new Random();

        public static T[] Shuffle<T>(this T[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                array[i] = array[Random.Next(i, array.Length)];
            }

            return array;
        }
    }
}