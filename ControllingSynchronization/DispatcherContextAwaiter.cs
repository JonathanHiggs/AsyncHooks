using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace ControllingSynchronization
{
    public struct DispatcherContextAwaiter : INotifyCompletion
    {
        private readonly Dispatcher dispatcher;


        public DispatcherContextAwaiter(Dispatcher dispatcher)
            => this.dispatcher = dispatcher;


        public bool IsCompleted => dispatcher.CheckAccess();


        public void OnCompleted(Action continuation)
            => dispatcher.BeginInvoke(continuation);


        public void GetResult() { }


        public DispatcherContextAwaiter GetAwaiter() => this;
    }
}
