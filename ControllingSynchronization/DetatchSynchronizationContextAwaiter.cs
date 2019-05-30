using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ControllingSynchronization
{
    public struct DetatchSynchronizationContextAwaiter : ICriticalNotifyCompletion
    {
        /// <summary>
        /// Returns true if the current synchronization context is null
        /// The continuation is called only when the current context is presented
        /// Close to 0 overhead if not already on a synchronization context
        /// </summary>
        public bool IsCompleted => SynchronizationContext.Current is null;


        public void OnCompleted(Action continuation)
            => ThreadPool.QueueUserWorkItem(_ => continuation());


        public void UnsafeOnCompleted(Action continuation)
            => ThreadPool.UnsafeQueueUserWorkItem(_ => continuation(), null);


        public void GetResult() { }


        public DetatchSynchronizationContextAwaiter GetAwaiter() => this;
    }
}
