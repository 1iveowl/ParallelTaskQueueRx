using System;
using System.Collections.Generic;
using System.Text;
using IParallelTaskQueueRx;

namespace ParallelTaskQueueRx
{
    public class ParallelTaskQueueRx<TReturnValue> : IParallelTaskQueueRx<TReturnValue>
    {
        public IObservable<TReturnValue> ObservableResults => throw new NotImplementedException();

        public void ProcessTaskOnMyQueue(
            Func<System.Threading.Tasks.Task<TReturnValue>> myTask, 
            TReturnValue errorReturnValue, 
            string queueId)
        {
            throw new NotImplementedException();
        }
    }
}
