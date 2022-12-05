namespace ThreadPool;

public class MyTask<TResult> : IMyTask<TResult>
{
    public bool IsCompleted { get; private set; }
    private TResult CalculatedValue { get; set; }
    private readonly MyThreadPool _threadPool;
    private readonly Func<TResult> _supplier;
    private readonly object _locker = new();

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

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newFunc)
    {
        return _threadPool.Submit(() => newFunc(Result));
    }
}