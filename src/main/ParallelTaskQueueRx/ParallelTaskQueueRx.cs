using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using IParallelTaskQueueRx;

namespace ParallelTaskQueueRx
{
    public class ParallelTaskQueueRx<TReturnValue> : IParallelTaskQueueRx<TReturnValue>
    {
        private readonly ISubject<TReturnValue> _subjectReturnValue = new Subject<TReturnValue>();
        private IObserver<TReturnValue> ObserverReturnValue => _subjectReturnValue.AsObserver();

        private readonly IDictionary<string, BlockingCollection<Func<Task<TReturnValue>>>> _queueDirectory =
            new ConcurrentDictionary<string, BlockingCollection<Func<Task<TReturnValue>>>>();

        public IObservable<TReturnValue> ObservableResults => _subjectReturnValue.AsObservable();

        public void ProcessTaskOnMyQueue(
            Func<Task<TReturnValue>> myTask, 
            TReturnValue errorReturnValue, 
            string queueId)
        {
            if (!_queueDirectory.ContainsKey(queueId))
            {
                Debug.WriteLine($"Creating Queue: {queueId}");

                _queueDirectory.Add(queueId, new BlockingCollection<Func<Task<TReturnValue>>>());

                _queueDirectory[queueId].Add(myTask);

                ProcessQueue(queueId, errorReturnValue);
            }
            else
            {
                Debug.WriteLine($"Adding value to queue: {queueId}");
                _queueDirectory[queueId].Add(myTask);
            }
        }

        private void ProcessQueue(string queueId, TReturnValue errorReturnValue)
        {
            Task.Run(async () =>
            {
                while (_queueDirectory[queueId].Count > 0)
                {
                    try
                    {
                        var task = _queueDirectory[queueId].Take();
                        var result = await task();

                        lock (ObserverReturnValue)
                        {
                            ObserverReturnValue.OnNext(result);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        lock (ObserverReturnValue)
                        {
                            ObserverReturnValue.OnNext(errorReturnValue);
                        }
                    }
                }

                _queueDirectory[queueId].CompleteAdding();

                lock (_queueDirectory)
                {
                    _queueDirectory.Remove(queueId);
                }

                Debug.WriteLine($"Removing Queue: {queueId}");

            });
        }

        public void Dispose()
        {
            _queueDirectory.Clear();

            ObserverReturnValue.OnCompleted();
        }
    }
}
