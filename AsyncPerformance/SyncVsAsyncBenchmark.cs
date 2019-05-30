using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace AsyncPerformance
{
    [Config(typeof(Config))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class SyncVsAsyncBenchmark
    {
        private readonly StockPrices stockPrices = new StockPrices();

        public SyncVsAsyncBenchmark()
        {
            // Warm up the cache
            stockPrices.GetStockPriceFor("MSFT");
        }


        [Benchmark]
        public decimal GetPricesDirectlyFromCache()
        {
            return stockPrices.GetPricefromCacheFor("MSFT");
        }


        [Benchmark]
        public decimal GetStockPriceFor()
        {
            return stockPrices.GetStockPriceFor("MSFT");
        }


        [Benchmark]
        public decimal GetStockPriceForAsync()
        {
            return stockPrices.GetStockPriceForAsync("MSFT").GetAwaiter().GetResult();
        }


        [Benchmark]
        public decimal GetStockPriceForAsync_ValueTask()
        {
            return stockPrices.GetStockPriceForAsync_ValueTask("MSFT").GetAwaiter().GetResult();
        }


        [Benchmark]
        public decimal GetStockPriceForAsync_Optimized()
        {
            return stockPrices.GetStockPriceForAsync_Optimized("MSFT").GetAwaiter().GetResult();
        }


        [Benchmark]
        public decimal GetStockPriceForAsync_OptimizedReverse()
        {
            return stockPrices.GetStockPriceForAsync_OptimizedReversed("MSFT").GetAwaiter().GetResult();
        }


        [Benchmark]
        public decimal GetStockPriceForAsync_OptimizedReverse_Await()
        {
            return stockPrices.GetStockPriceForAsync_OptimizedReversed_Await("MSFT").GetAwaiter().GetResult();
        }


        private class Config : ManualConfig
        {
            public Config()
            {
                Add(Job.MediumRun
                    .WithLaunchCount(1)
                    .With(InProcessEmitToolchain.Instance)
                    .WithId("InProcess"));
            }
        }
    }
}
