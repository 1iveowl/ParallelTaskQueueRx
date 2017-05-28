using System;
using System.Threading.Tasks;

namespace IParallelTaskQueueRx
{
    public interface IParallelTaskQueueRx<TReturnValue>
    {
        IObservable<TReturnValue> ObservableResults { get; }

        void ProcessTaskOnMyQueue(
            Func<Task<TReturnValue>> myTask,
            TReturnValue errorReturnValue,
            string queueId);
    }
}
