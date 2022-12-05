namespace ThreadPool;

/// <summary>
/// Thread pool task class
/// </summary>
/// <typeparam name="TResult">Type of the result</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    public bool IsCompleted { get; private set; }
    private TResult CalculatedValue { get; set; }
    private readonly MyThreadPool _threadPool;
    private readonly Func<TResult> _supplier;
    private readonly object _locker = new();

    /// <summary>
    /// Returns the result value
    /// </summary>
    public TResult Result
    {
        get
        {
            lock (_locker)
            {
                if (!IsCompleted) CalculateResult();
                return CalculatedValue;
            }
        }
    }


    public MyTask(MyThreadPool myThreadPool, Func<TResult> supplier)
    {
        _threadPool = myThreadPool;
        _supplier = supplier;
    }

    /// <summary>
    /// Calculates the result
    /// </summary>
    public void CalculateResult()
    {
        try
        {
            CalculatedValue = _supplier();
            IsCompleted = true;
        }
        catch (Exception e)
        {
            throw new AggregateException(e);
        }
    }

    /// <summary>
    /// Create a new task, which continues the current
    /// </summary>
    /// <param name="newFunc">Function of the new task</param>
    /// <typeparam name="TNewResult">Type of the result of the new task</typeparam>
    /// <returns></returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newFunc)
    {
        return _threadPool.Submit(() => newFunc(Result));
    }
}