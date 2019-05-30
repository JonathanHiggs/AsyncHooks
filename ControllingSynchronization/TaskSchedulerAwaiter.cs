using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ControllingSynchronization
{
    public struct TaskSchedulerAwaiter : INotifyCompletion
    {
        public bool IsCompleted => SynchronizationContext.Current is null;


        public void OnCompleted(Action continuation)
            => Task.Run(continuation);


        public void GetResult() { }


        public TaskSchedulerAwaiter GetAwaiter() => this;
    }
}
