# Parallel Task Queue Rx

[![NuGet Badge](https://buildstats.info/nuget/ParallelTaskQueueRx)](https://www.nuget.org/packages/ParallelTaskQueueRx)

[![.NET Standard](http://img.shields.io/badge/.NET_Standard-v1.2-green.svg)](https://docs.microsoft.com/da-dk/dotnet/articles/standard/library)

## Please

Please star this project if you find it useful. Thank you.

## What is this?

This is a simple .NET Standard 1.2 NuGet that makes it easy to run multiple task queues in parallel and observe their return values on a single observable. 

Each queues is given it's own queue id (a string).  

## How do I use this?

Like this:

```csharp
class Program
{
    static void Main(string[] args)
    {
        var parallelTaskQueue = new ParallelTaskQueueRx<string>();

        var disposable = parallelTaskQueue.ObservableResults.Subscribe(
            System.Console.WriteLine,
            ex => {System.Console.WriteLine($"Error: {ex.Message}");},
            () => {System.Console.WriteLine("Compleded");});

        Start(parallelTaskQueue);

        System.Console.ReadKey();

        disposable.Dispose();
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
            },"Queue1");

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

```

## More
There is a discussion about this project [here](https://codereview.stackexchange.com/questions/164393/generic-parallel-task-queue-returning-an-observable-sequence-of-return-value). 