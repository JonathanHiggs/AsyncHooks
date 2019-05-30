using System.Windows.Threading;

namespace ControllingSynchronization
{
    public static class Awaiters
    {
        public static DetatchSynchronizationContextAwaiter DetatchSynchronizationContext()
            => default;


        public static DispatcherContextAwaiter DispatcherContext(Dispatcher dispatcher)
            => new DispatcherContextAwaiter(dispatcher);


        public static TaskSchedulerAwaiter TaskSchedulerContext()
            => default;
    }
}
