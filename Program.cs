using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WordlessSearch
{
    class Program
    {
        private static List<(Task<WordlessSearch>, CancellationTokenSource)> tasks =
            new List<(Task<WordlessSearch>, CancellationTokenSource)>(Constants.Threads);

        private static void Prepare()
        {
            Console.SetWindowSize(Console.WindowWidth, Math.Max(Console.WindowHeight, Constants.GridSize + 2));

            Console.CursorVisible = false;
            Console.Clear();
        }

        static void Finish(WordlessSearch grid)
        {
            Console.Clear();
            grid.Print();

            Console.CursorVisible = true;
            Console.Beep(440, 1000);
        }

        static async Task Main(string[] args)
        {
            Prepare();

            WordlessSearch grid;

            if (!Constants.RunParallel)
            {
                CancellationTokenSource dummy = new CancellationTokenSource();
                grid = RunWordlessSearch(dummy.Token, dummy.Token);
            }
            else
            {
                CancellationTokenSource finishedTokenSource = new CancellationTokenSource();
                // PushTask(finishedTokenSource.Token);
                // PrintTaskCount();

                for (int i = 0; i < Constants.Threads; i++)
                {
                    PushTask(finishedTokenSource.Token);
                }

                grid = await await Task.WhenAny(tasks.Select(v => v.Item1));
                finishedTokenSource.Cancel();

                // while (true)
                // {
                //     using (Task task = await Task.WhenAny(
                //         tasks.Select(v => v.Item1 as Task)
                //             .Append(Task.Run(() => HandleConsole(finishedTokenSource.Token)))))
                //     {
                //         Task<WordlessSearch> gridTask = task as Task<WordlessSearch>;
                //         Task<Nullable<(Task<WordlessSearch>, CancellationTokenSource)>> cancelTask =
                //             task as Task<Nullable<(Task<WordlessSearch>, CancellationTokenSource)>>;
                //         if (gridTask != null && !task.IsCanceled && task.IsCompletedSuccessfully)
                //         {
                //             grid = await gridTask;
                //             gridTask.Dispose();
                //             finishedTokenSource.Cancel();
                //             break;
                //         }
                //         else if (cancelTask != null && !cancelTask.IsCanceled && cancelTask.Result != null)
                //         {
                //             var (canceledTask, cancelToken) = (await cancelTask).Value;
                //             cancelToken.Cancel();
                //             using (canceledTask)
                //             {
                //                 try
                //                 {
                //                     await canceledTask;
                //                 }
                //                 catch (TaskCanceledException) {};
                //             }
                //         }
                //     }
                // }
            }

            Finish(grid);
        }

        static async Task<Nullable<(Task<WordlessSearch>, CancellationTokenSource)>> HandleConsole(CancellationToken finishedToken)
        {
            while (!finishedToken.IsCancellationRequested)
            {
                Nullable<(Task<WordlessSearch>, CancellationTokenSource)> value = null;
                while (!finishedToken.IsCancellationRequested && !Console.KeyAvailable)
                {
                    await Task.Yield();
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        PushTask(finishedToken);
                        break;

                    case ConsoleKey.DownArrow:
                        value = PopTask();
                        if (value == null)
                        {
                            continue;
                        }

                        break;

                    default:
                        continue;
                }

                PrintTaskCount();
                return value;
            }

            throw new TaskCanceledException();
        }

        static void PrintTaskCount()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Running tasks: {string.Format("{0,2:#0}", tasks.Count)}");
        }

        static void PushTask(CancellationToken finishedToken)
        {
            lock (tasks)
            {
                if (tasks.Count < Environment.ProcessorCount - 1)
                {
                    CancellationTokenSource taskTokenSource = new CancellationTokenSource();
                    tasks.Add((Task.Run(() => RunWordlessSearch(finishedToken, taskTokenSource.Token)), taskTokenSource));
                }
            }
        }

        static Nullable<(Task<WordlessSearch>, CancellationTokenSource)> PopTask()
        {
            lock (tasks)
            {
                if (tasks.Count > 0)
                {
                    (Task<WordlessSearch>, CancellationTokenSource) value = tasks.Last();
                    tasks.RemoveAt(tasks.Count - 1);
                    return value;
                }
                else
                {
                    return null;
                }
            }
        }

        static WordlessSearch RunWordlessSearch(CancellationToken finishedToken, CancellationToken taskToken)
        {
            WordlessSearch grid = new WordlessSearch();

            grid.DoPrint = !Constants.RunParallel && Constants.DoPrint;

            grid.DoEvil(finishedToken, taskToken);
            return grid;
        }
    }
}
