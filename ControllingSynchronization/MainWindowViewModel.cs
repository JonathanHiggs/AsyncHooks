using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ControllingSynchronization
{
    class MainWindowViewModel
    {
        private const int delay = 1500;


        public async Task<decimal> DoWork()
        {
            await Task.Delay(delay);
            return 42;
        }

        public async Task<decimal> DoWork_ConfigureAwait()
        {
            await Task.Delay(delay).ConfigureAwait(false);
            return 42;
        }

        public async Task<decimal> DoWork_WithDetatch()
        {
            // All public async methods should detatch as the first op
            // Should write a roserlyn analyzer to detect that
            await Awaiters.DetatchSynchronizationContext();
            await Task.Delay(delay);
            return 42;
        }

        public async Task<decimal> DoWork_WithTaskScheduler()
        {
            // All public async methods should detatch as the first op
            // Should write a roserlyn analyzer to detect that
            await Awaiters.TaskSchedulerContext();
            Debug.WriteLine(SynchronizationContext.Current);
            await Task.Delay(delay);
            Debug.WriteLine(SynchronizationContext.Current);
            return 42;
        }
    }
}
