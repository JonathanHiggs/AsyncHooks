using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHooks
{
    class Program
    {
        // https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/
        
        static async Task Main(string[] args)
        {
            Console.WriteLine($"StockPriceFor: {await StockPriceForMsft()}\n");
            Console.WriteLine($"StockPriceFor_BasicStateMachine: {await StockPriceForMsft_BasicStateMachine()}\n");
            Console.WriteLine($"StockPriceFor_AsyncStateMachine: {await StockPriceForMsft_AsyncStateMachine()}\n");

            await ExecutionContextInAction();
            await ExecutionContextInAsyncMethod();

            RunAsyncVoid();

            await UseLazyAwaiter();

            await SwitchExecutionToTaskThreadPool();

            await UseTaskLikeWrapper();
        }


        static Task<decimal> StockPriceForMsft()
        {
            var stockPrices = new StockPrices();
            return stockPrices.StockPriceForAsync("MSFT");
        }


        static Task<decimal> StockPriceForMsft_BasicStateMachine()
        {
            var stockPrices = new StockPrices();
            return stockPrices.StockPricesForAsync_BasicStateMachine("MSFT");
        }


        static Task<decimal> StockPriceForMsft_AsyncStateMachine()
        {
            var stockPrices = new StockPrices();
            return stockPrices.StockPricesForAsync_AsyncStateMachine("MSFT");
        }


        static Task ExecutionContextInAction()
        {
            Console.WriteLine("- ExecutionContextInAction -");
            var asyncLocal = new AsyncLocal<int> { Value = 42 };

            return Task.Run(() =>
            {
                Console.WriteLine($"In Task.Run: {asyncLocal.Value} [{Thread.CurrentThread.ManagedThreadId}]");
            }).ContinueWith(_ =>
            {
                Console.WriteLine($"In Task.ContinueWith: {asyncLocal.Value} [{Thread.CurrentThread.ManagedThreadId}]\n");
            });
        }


        static async Task ExecutionContextInAsyncMethod()
        {
            Console.WriteLine("- ExecutionContextInAsyncMethod -");
            var asyncLocal = new AsyncLocal<int> { Value = 42 };

            await Task.Delay(42);

            // Execution context is implicitly captured
            Console.WriteLine($"After first await: {asyncLocal.Value} [{Thread.CurrentThread.ManagedThreadId}]");

            var task = Task.Yield();
            task.GetAwaiter().UnsafeOnCompleted(() =>
            {
                // Execution context is not streamed by UnsafeOnCompleted
                Console.WriteLine($"Inside UnsafeOnComplete {asyncLocal.Value} [{Thread.CurrentThread.ManagedThreadId}]");
            });

            await task;

            Console.WriteLine($"After second await: {asyncLocal.Value} [{Thread.CurrentThread.ManagedThreadId}]\n");
        }


        static void RunAsyncVoid()
        {
            Console.WriteLine("- RunAsyncVoid -");
            Console.WriteLine("Before VoidAsync");

            VoidAsync();

            Console.WriteLine("After VoidAsync\n");
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async void VoidAsync()
        {
            Console.WriteLine("Inside VoidAsync()");
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously


        static async Task UseLazyAwaiter()
        {
            Console.WriteLine("- UseLazyAwaiter -");
            var lazy = new Lazy<int>(() => {
                Console.WriteLine("Inside Lazy initialization");
                return 42;
            });
            var result = await lazy;
            Console.WriteLine($"LazyAwaiter result: {result}\n");
        }


        static async Task SwitchExecutionToTaskThreadPool()
        {
            Console.WriteLine("- SwitchExecutionToTaskThreadPool -");
            Console.WriteLine($"Starting thread: {Thread.CurrentThread.ManagedThreadId}");
            await Sample();
            Console.WriteLine($"After awaiting thread: {Thread.CurrentThread.ManagedThreadId}\n");

            async Task Sample()
            {
                await default(SwitchToTaskThreadPool);
                Console.WriteLine($"Inside Sample thread: {Thread.CurrentThread.ManagedThreadId}");
            }
        }


        static async Task UseTaskLikeWrapper()
        {
            Console.WriteLine("- UseTaskLike -");
            await UseTaskLike();
            Console.WriteLine();
        }
        
        static async TaskLike UseTaskLike()
        {
            await Task.Yield();
            await default(TaskLike);
        }
    }
}
