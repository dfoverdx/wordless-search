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
            // grid.Verbose = false;
            grid.DoEvil();

            Console.Clear();
            grid.Print();

            Console.CursorVisible = true;
            Console.Beep(440, 2000);
        }
    }
}
