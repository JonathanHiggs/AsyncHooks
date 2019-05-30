using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AsyncHooks
{
    /// <summary>
    /// Defines an awaitable that can be used to schedule the continuation on the task threadpool
    /// Could switch to wpf.Dispatcher by changing the OnCompleted method to Dispatcher.Invoke
    /// </summary>
    public struct SwitchToTaskThreadPool : INotifyCompletion
    {
        public SwitchToTaskThreadPool GetAwaiter() => this;
        public bool IsCompleted => false;
        
        public void OnCompleted(Action continuation) => Task.Run(continuation);
        public void GetResult() { }
    }
}
