namespace ThreadPool;

public class MyTask<TResult> : IMyTask<TResult>
{
    public bool IsCompleted { get; private set; }
    private TResult CalculatedValue { get; set; }
    private readonly MyThreadPool _threadPool;

    public TResult Result
    {
        get
        {
            if (!IsCompleted) CalculateResult();
            return CalculatedValue;
        }
    }

    private readonly Func<TResult> _supplier;
    private readonly object _locker = new();

    public MyTask(MyThreadPool myThreadPool, Func<TResult> supplier)
    {
        _threadPool = myThreadPool;
        _supplier = supplier;
    }

    public void CalculateResult()
    {
        lock (_locker)
        {
            try
            {
                CalculatedValue = _supplier();
            }
            catch (Exception e)
            {
                throw new AggregateException(e);
            }

            IsCompleted = true;
        }
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newFunc)
    {
        return _threadPool.Submit(() => newFunc(Result));
    }
}