using System;
using BenchmarkDotNet.Running;

namespace AsyncPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<SyncVsAsyncBenchmark>();
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
