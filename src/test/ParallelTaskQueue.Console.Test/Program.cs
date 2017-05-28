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
                System.Console.WriteLine,
                ex => {System.Console.WriteLine($"Error: {ex.Message}");},
                () => {System.Console.WriteLine("Compleded");});

            Start(parallelTaskQueue);

            System.Console.ReadKey();
        }

        private static async void Start(ParallelTaskQueueRx<string> parallelTaskQueue)
        {
            parallelTaskQueue.ProcessTaskOnSpecificQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    return "Queue1: #1 (1 sec delay)";
                },"Queue1");

            parallelTaskQueue.ProcessTaskOnSpecificQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "Queue2: #1 (0,25 sec delay)";
                },"Queue2");

            System.Console.WriteLine("----Waiting 0,25 sec----");
            await Task.Delay(TimeSpan.FromMilliseconds(250));

            parallelTaskQueue.ProcessTaskOnSpecificQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "Queue1: #2 (0,25 sec delay)";
                },"Queue 1");

            parallelTaskQueue.ProcessTaskOnSpecificQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1250));
                    return "Queue2 2 (1,25 sec delay)";
                },"Queue2");

            parallelTaskQueue.ProcessTaskOnSpecificQueue(() => Task.FromResult("Queue2 #3 (no delay)"),"Queue2");

            parallelTaskQueue.ProcessTaskOnSpecificQueue(() => Task.FromResult("Queue2 #4 (no delay)"),"Queue2");

            System.Console.WriteLine("----Waiting 5 sec----");
            await Task.Delay(TimeSpan.FromSeconds(5));

            parallelTaskQueue.ProcessTaskOnSpecificQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    return "Queue1 #3 (1 sec delay)";
                },"Queue1");

            parallelTaskQueue.ProcessTaskOnSpecificQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "Queue2 #5 (0,25 sec delay)";
                },"Queue2");

            System.Console.WriteLine("----Waiting 0,25 sec----");
            await Task.Delay(TimeSpan.FromMilliseconds(250));

            parallelTaskQueue.ProcessTaskOnSpecificQueue(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    return "Queue1 #4 (0,25 sec delay)";
                },"Queue1");
        }
    }
}