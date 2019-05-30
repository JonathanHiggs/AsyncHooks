using System;
using System.Runtime.CompilerServices;
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


    public static class DispatcherExtensions
    {
        /// <summary>
        /// Enables awaiting on an instance of the dispatcher to switch synchronization context
        /// </summary>
        /// <example>
        /// await myControl.Dispatcher;
        /// </example>
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        public static DispatcherContextAwaiter GetAwaiter(this Dispatcher dispatcher)
            => new DispatcherContextAwaiter(dispatcher);
    }
}
