using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
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

        public void ProcessTaskOnSpecificQueue(
            Func<Task<TReturnValue>> myTask, 
            string queueId)
        {

            if (!_queueDirectory.ContainsKey(queueId))
            {
                Debug.WriteLine($"Creating Queue: {queueId}");

                _queueDirectory.Add(queueId, new BlockingCollection<Func<Task<TReturnValue>>>());

                _queueDirectory[queueId].Add(myTask);

                ProcessQueue(queueId);
            }
            else
            {
                Debug.WriteLine($"Adding value to queue: {queueId}");
                _queueDirectory[queueId].Add(myTask);
            }
        }

        private void ProcessQueue(string queueId)
        {
            Task.Run(async () =>
            {
                while (_queueDirectory[queueId].Count > 0)
                {
                    var task = _queueDirectory[queueId].Take();
                    TReturnValue result;

                    try
                    {
                        result = await task();
                    }
                    catch (Exception ex)
                    {
                        lock (ObserverReturnValue)
                        {
                            ObserverReturnValue.OnError(ex);
                            continue;
                        }
                    }

                    lock (ObserverReturnValue)
                    {
                        ObserverReturnValue.OnNext(result);
                    }
                }

                _queueDirectory[queueId].CompleteAdding();
                _queueDirectory.Remove(queueId);

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
