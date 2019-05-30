using System;
using System.Runtime.CompilerServices;

namespace AsyncHooks
{
    public sealed class TaskLikeMethodBuilder
    {
        public TaskLikeMethodBuilder() =>
            Console.WriteLine("TaskLikeMethodBuilder.ctor");


        public static TaskLikeMethodBuilder Create() =>
            new TaskLikeMethodBuilder();


        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }


        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }


        public void SetException(Exception exception) { }


        public void SetResult() => Console.WriteLine("SetResult");


        public void SetStateMachine(IAsyncStateMachine stateMachine) { }


        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            Console.WriteLine($"Start: {typeof(TStateMachine).Name}");
            stateMachine.MoveNext();
        }

        public TaskLike Task => default;
    }
}
