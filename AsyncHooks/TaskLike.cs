using System.Runtime.CompilerServices;

namespace AsyncHooks
{
    [AsyncMethodBuilder(typeof(TaskLikeMethodBuilder))]
    public struct TaskLike
    {
        public TaskLikeAwaiter GetAwaiter() => default;
    }
}
