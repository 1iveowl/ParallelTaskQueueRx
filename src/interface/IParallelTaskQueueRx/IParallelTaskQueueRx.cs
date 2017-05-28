using System;
using System.Threading.Tasks;

namespace IParallelTaskQueueRx
{
    public interface IParallelTaskQueueRx<TReturnValue> : IDisposable
    {
        IObservable<TReturnValue> ObservableResults { get; }

        void ProcessTaskOnMyQueue(
            Func<Task<TReturnValue>> myTask,
            string queueId);
    }
}
