using System;
using System.Threading.Tasks;
using IParallelTaskQueueRx;
using ParallelTaskQueueRx;

namespace ParallelTaskQueue.Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var parallelTaskQueue = new ParallelTaskQueueRx<string>();

            var disp = parallelTaskQueue.ObservableResults.Subscribe(
                s =>
                {
                    System.Console.WriteLine(s);

                },
                ex => {System.Console.WriteLine($"Error: {ex.Message}");},
                () => {System.Console.WriteLine("Compleded");});

            Start(parallelTaskQueue);

            System.Console.ReadKey();

        }

        private static async void Start(ParallelTaskQueueRx<string> parallelTaskQueue)
        {
            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    return "1.1";
                }, 
                "Error",
                "1");

            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "2.1";
                },
                "Error",
                "2");

            await Task.Delay(TimeSpan.FromMilliseconds(250));

            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "1.2";
                },
                "Error",
                "1");

            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1250));
                    return "2.2";
                },
                "Error",
                "2");

            parallelTaskQueue.ProcessTaskOnMyQueue(() =>
                {
                    return Task.FromResult("2.3");

                },
                "Error",
                "2");
            parallelTaskQueue.ProcessTaskOnMyQueue(() =>
                {
                    return Task.FromResult("2.4");
                },
                "Error",
                "2");

            await Task.Delay(TimeSpan.FromSeconds(5));

            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    return "1.3";
                },
                "Error",
                "1");

            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "2.5";
                },
                "Error",
                "2");

            await Task.Delay(TimeSpan.FromMilliseconds(250));

            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "1.4";
                },
                "Error",
                "1");

            parallelTaskQueue.ProcessTaskOnMyQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1250));
                    return "2.6";
                },
                "Error",
                "2");

            parallelTaskQueue.ProcessTaskOnMyQueue(() =>
                {
                    return Task.FromResult("2.6");

                },
                "Error",
                "2");
            parallelTaskQueue.ProcessTaskOnMyQueue(() =>
                {
                    return Task.FromResult("2.8");
                },
                "Error",
                "2");

        }
    }
}