using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHooks
{
    class StockPrices
    {
        #region OriginalClass

        private Dictionary<string, decimal> stockPrices;
        

        public async Task<decimal> StockPriceForAsync(string companyId)
        {
            if (string.IsNullOrEmpty(companyId))
                throw new ArgumentNullException(nameof(companyId));

            await InitializeMapIfNeededAsync();

            stockPrices.TryGetValue(companyId, out var result);
            return result;
        }

        private async Task InitializeMapIfNeededAsync()
        {
            if (!(stockPrices is null))
                return;

            // Simulates receiving the stock prices from the external source and cache in memory
            await Task.Delay(42);
            stockPrices = new Dictionary<string, decimal> { { "MSFT", 42 } };
        }

        #endregion


        #region BasicStateMachine
        

        /// <summary>
        /// Executes the same logic as the original call with the basic state machine
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Task<decimal> StockPricesForAsync_BasicStateMachine(string companyId)
        {
            var stateMachine = new StockPricesForAsync__BasicStateMachine(this, companyId);
            stateMachine.Start();
            return stateMachine.Task;
        }


        /// <summary>
        /// Basic example of a manually rolled async state machine for <see cref="StockPriceForAsync(string)"/>
        /// </summary>
        class StockPricesForAsync__BasicStateMachine
        {
            enum State { Start, Step1 }

            private readonly StockPrices @this;
            private readonly string companyId;
            private readonly TaskCompletionSource<decimal> tcs;
            private Task initializeMapIfNeededTask;
            private State state = State.Start;

            public StockPricesForAsync__BasicStateMachine(StockPrices @this, string companyId)
            {
                this.@this = @this;
                this.companyId = companyId;
                tcs = new TaskCompletionSource<decimal>();
            }

            public void Start()
            {
                try
                {
                    if (state == State.Start)
                    {
                        // Code from the first block of the original method, until the await
                        if (string.IsNullOrEmpty(companyId))
                            throw new ArgumentNullException(nameof(companyId));

                        initializeMapIfNeededTask = @this.InitializeMapIfNeededAsync();

                        // Update state and schedule continuation
                        state = State.Step1;
                        initializeMapIfNeededTask.ContinueWith(_ => Start());
                    }
                    else if (state == State.Step1)
                    {
                        // Need to check the error and cancel case first
                        if (initializeMapIfNeededTask.Status == TaskStatus.Canceled)
                            tcs.SetCanceled();
                        else if (initializeMapIfNeededTask.Status == TaskStatus.Faulted)
                            tcs.SetException(initializeMapIfNeededTask.Exception.InnerException);
                        else
                        {
                            // Code from the second block of the original method, from after the await
                            @this.stockPrices.TryGetValue(companyId, out var result);
                            tcs.SetResult(result);
                        }
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }

            public Task<decimal> Task => tcs.Task;
        }

        #endregion


        #region AsyncStateMachine

        /// <summary>
        /// Executes the same logic as the original methods with the optimized state machine
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [AsyncStateMachine(typeof(StockPricesForAsync__AsyncStateMachine))]
        public Task<decimal> StockPricesForAsync_AsyncStateMachine(string companyId)
        {
            StockPricesForAsync__AsyncStateMachine stockPriceForAsync = new StockPricesForAsync__AsyncStateMachine(); ;
            stockPriceForAsync.@this = this;
            stockPriceForAsync.companyId = companyId;
            stockPriceForAsync.builder = AsyncTaskMethodBuilder<decimal>.Create();
            stockPriceForAsync.state = -1;
            var builder = stockPriceForAsync.builder;
            builder.Start(ref stockPriceForAsync);
            return stockPriceForAsync.builder.Task;
        }


        /// <summary>
        /// Optimized version of the state machine
        /// 1. Struct and passed arround by value
        /// 2. Hot path optimiaztion - checks if the task is already completed to return sections synchronously
        /// </summary>
        struct StockPricesForAsync__AsyncStateMachine : IAsyncStateMachine
        {
            public StockPrices @this;
            public string companyId;
            public AsyncTaskMethodBuilder<decimal> builder;
            public int state;
            private TaskAwaiter task1Awaiter;

            public void MoveNext()
            {
                decimal result;
                try
                {
                    TaskAwaiter awaiter;
                    if (state != 0)
                    {
                        // State 1 of the generated state machine
                        if (string.IsNullOrEmpty(companyId))
                            throw new ArgumentNullException(nameof(companyId));

                        awaiter = @this.InitializeMapIfNeededAsync().GetAwaiter();

                        // Hot path optimisation: if the task is completed the state machine
                        // automatically moves to the next step
                        if (!awaiter.IsCompleted)
                        {
                            state = 0;
                            task1Awaiter = awaiter;

                            // This will eventually cause boxing of the state machine == heap allocation :(
                            builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = task1Awaiter;
                        task1Awaiter = default;
                        state = -1;
                    }

                    // GetResult returns void, but it'll throw if the awaited task failed
                    // This exception is caught later and changes the resulting task
                    awaiter.GetResult();
                    @this.stockPrices.TryGetValue(companyId, out result);
                }
                catch (Exception exception)
                {
                    // Final state: failure
                    state = -2;
                    builder.SetException(exception);
                    return;
                }

                // Final state: success
                state = -2;
                builder.SetResult(result);
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                builder.SetStateMachine(stateMachine);
            }
        }

        #endregion
    }
}
