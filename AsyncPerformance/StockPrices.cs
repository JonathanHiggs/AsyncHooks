using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncPerformance
{
    public class StockPrices
    {
        private const int Count = 1;
        private List<(string Name, decimal Price)> stockPricesCache;


        // Async version
        public async Task<decimal> GetStockPriceForAsync(string companyId)
        {
            await InitializeMapIfNeededAsync();
            return DoGetPriceFromCache(companyId);
        }


        // Async with value task doesn't allocate to the heap
        public async ValueTask<decimal> GetStockPriceForAsync_ValueTask(string companyId)
        {
            await InitializeMapIfNeededAsync();
            return DoGetPriceFromCache(companyId);
        }


        // Async with value task and optimize to avoid async state machine on hot path
        // i.e. no async keyword, just returns a ValueTask
        public ValueTask<decimal> GetStockPriceForAsync_Optimized(string companyId)
        {
            var task = InitializeMapIfNeededAsync_Optimized();

            if (!task.IsCompleted)
                return DoGetStockPriceForAsync(task, companyId);
            return new ValueTask<decimal>(DoGetPriceFromCache(companyId));

            // without passing in the variables the function would create a closure that involves allocations
            // if c#8 should use static local function to prevent the possibility of any allocations 
            async ValueTask<decimal> DoGetStockPriceForAsync(Task initializeTask, string localCompanyId)
            {
                await initializeTask;
                return DoGetPriceFromCache(localCompanyId);
            }
        }


        // Async with value task and optimize to avoid async state machine on hot path
        // i.e. no async keyword, just returns a ValueTask
        public ValueTask<decimal> GetStockPriceForAsync_OptimizedReversed(string companyId)
        {
            var task = InitializeMapIfNeededAsync();

            if (task.IsCompleted)
                return new ValueTask<decimal>(DoGetPriceFromCache(companyId));
            return DoGetStockPriceForAsync(task, companyId);

            // without passing in the variables the function would create a closure that involves allocations
            // if c#8 should use static local function to prevent the possibility of any allocations 
            async ValueTask<decimal> DoGetStockPriceForAsync(Task initializeTask, string localCompanyId)
            {
                await initializeTask;
                return DoGetPriceFromCache(localCompanyId);
            }
        }


        // Async with value task and optimize to avoid async state machine on hot path
        // i.e. no async keyword, just returns a ValueTask
        public ValueTask<decimal> GetStockPriceForAsync_OptimizedReversed_Await(string companyId)
        {
            var task = InitializeMapIfNeededAsync_AlwaysAwait();

            if (task.IsCompleted)
                return new ValueTask<decimal>(DoGetPriceFromCache(companyId));
            return DoGetStockPriceForAsync(task, companyId);

            // without passing in the variables the function would create a closure that involves allocations
            // if c#8 should use static local function to prevent the possibility of any allocations 
            async ValueTask<decimal> DoGetStockPriceForAsync(Task initializeTask, string localCompanyId)
            {
                await initializeTask;
                return DoGetPriceFromCache(localCompanyId);
            }
        }


        // Sync version that calls async init
        public decimal GetStockPriceFor(string companyId)
        {
            InitializeMapIfNeededAsync().GetAwaiter().GetResult();
            return DoGetPriceFromCache(companyId);
        }


        // Puerly sync version
        public decimal GetPricefromCacheFor(string companyId)
        {
            InitializeMapIfNeeded();
            return DoGetPriceFromCache(companyId);
        }


        private decimal DoGetPriceFromCache(string companyId)
        {
            foreach (var kvp in stockPricesCache) // Intentionally a look to avoid allocations
                if (kvp.Name == companyId)
                    return kvp.Price;

            return decimal.Zero;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeMapIfNeeded()
        {
            if (!(stockPricesCache is null))
                return;

            Thread.Sleep(42);

            stockPricesCache = Enumerable.Range(1, Count)
                .Select(n => (n.ToString(), (decimal)n))
                .ToList();
            stockPricesCache.Add(("MSFT", 42));
        }


        private async Task InitializeMapIfNeededAsync()
        {
            if (!(stockPricesCache is null))
                return;

            await Task.Delay(42);

            stockPricesCache = Enumerable.Range(1, Count)
                .Select(n => (n.ToString(), (decimal)n))
                .ToList();
            stockPricesCache.Add(("MSFT", 42));
        }


        private async Task InitializeMapIfNeededAsync_AlwaysAwait()
        {
            if (!(stockPricesCache is null))
            {
                await Task.Yield();
                return;
            }

            await Task.Delay(42);

            stockPricesCache = Enumerable.Range(1, Count)
                .Select(n => (n.ToString(), (decimal)n))
                .ToList();
            stockPricesCache.Add(("MSFT", 42));
        }

        
        private Task InitializeMapIfNeededAsync_Optimized()
        {
            if (!(stockPricesCache is null))
                return Task.CompletedTask; // Avoids allocation

            return InitializeCache();

            async Task InitializeCache()
            {
                await Task.Delay(42);

                stockPricesCache = Enumerable.Range(1, Count)
                    .Select(n => (n.ToString(), (decimal)n))
                    .ToList();
                stockPricesCache.Add(("MSFT", 42));
            }
        }
    }
}
