namespace System.Collections.Generic
{
    public static class ArrayExtensions
    {
        private static Random Random = new Random();

        public static T[] Shuffle<T>(this T[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int idx = Random.Next(i, array.Length);
                (array[i], array[idx]) = (array[idx], array[i]);
            }

            return array;
        }
    }
}