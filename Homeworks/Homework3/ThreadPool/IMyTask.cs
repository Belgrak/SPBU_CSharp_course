namespace ThreadPool;

/// <summary>
/// Interface for thread pool tasks
/// </summary>
/// <typeparam name="TResult">Type of the result</typeparam>
public interface IMyTask<out TResult>
{
    public bool IsCompleted { get; }
    public TResult Result { get; }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask);
}