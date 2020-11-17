using System;
using System.Threading.Tasks;

namespace WordlessSearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.SetWindowSize(Console.WindowWidth, Math.Max(Console.WindowHeight, Constants.GridSize + 2));
            // Console.CursorVisible = false;
            Console.Clear();

            Grid grid = await await Task.WhenAny(
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid(),
                RunGrid()
            );

            Console.Clear();
            grid.Print();

            Console.CursorVisible = true;
            Console.Beep(440, 2000);
        }

        static async Task<Grid> RunGrid()
        {
            Grid grid = new Grid();
            grid.Verbose = false;
            grid.DoEvil();
            return grid;
        }
    }
}
