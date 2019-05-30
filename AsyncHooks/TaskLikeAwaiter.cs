using System;
using System.Runtime.CompilerServices;

namespace AsyncHooks
{
    public struct TaskLikeAwaiter : INotifyCompletion
    {
        public void GetResult() { }

        public bool IsCompleted => true;

        public void OnCompleted(Action continuation) { }
    }
}
