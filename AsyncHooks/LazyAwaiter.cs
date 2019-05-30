using System;
using System.Runtime.CompilerServices;

namespace AsyncHooks
{
    /// <summary>
    /// Custom awaitable that wraps a <see cref="Lazy{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct LazyAwaiter<T> : INotifyCompletion
    {
        private readonly Lazy<T> lazy;


        public LazyAwaiter(Lazy<T> lazy) => this.lazy = lazy;


        public T GetResult() => lazy.Value;


        public bool IsCompleted => true;


        public void OnCompleted(Action continuation) { }
    }


    public static class LazyAwaiterExtensions
    {
        /// <summary>
        /// Extension methods enables awaiting a <see cref="Lazy{T}"/> by using <see cref="LazyAwaiter{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lazy"></param>
        /// <returns></returns>
        public static LazyAwaiter<T> GetAwaiter<T>(this Lazy<T> lazy) =>
            new LazyAwaiter<T>(lazy);
    }
}
