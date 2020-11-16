using System;

namespace WordlessSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.WindowWidth, Math.Max(Console.WindowHeight, Constants.GridSize + 2));
            Console.CursorVisible = false;

            Grid grid = new Grid();
            grid.DoEvil();
            grid.Print();

            Console.CursorVisible = true;
        }
    }
}
