namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// MethodBuilder for async void method calls
    /// Creating a class of this name and namespace will 'hide' the default framework version
    /// To cover async Task and async Task<T> would need to implement AsyncTaskBuilder, and
    /// AsyncTaskMethodBuilderOfT
    /// </summary>
    public class AsyncVoidMethodBuilder
    {
        public AsyncVoidMethodBuilder()
            => Console.WriteLine(".ctor");


#pragma warning disable CS0436 // Type conflicts with imported type
        public static AsyncVoidMethodBuilder Create()
            => new AsyncVoidMethodBuilder();
#pragma warning restore CS0436 // Type conflicts with imported type


        public void AwaitOnCompleted<TAwaiter, TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }


        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine)
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
    }
}
