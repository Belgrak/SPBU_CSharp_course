using System.Collections.Concurrent;

namespace ThreadPool;

public class MyThreadPool
{
    private readonly ConcurrentQueue<Action> _queue;
    private readonly List<Thread> _threads;
    private readonly CancellationTokenSource _cts;

    /// <summary>
    /// Thread pool task class
    /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
    private class MyTask<TResult> : IMyTask<TResult>
    {
        public bool IsCompleted { get; private set; }
        private TResult CalculatedValue { get; set; }
        private readonly MyThreadPool _threadPool;
        private readonly Func<TResult> _supplier;
        private readonly object _locker = new();

        /// <summary>
        /// Returns the result value
        /// </summary>
        public TResult Result => GetResult();


        public MyTask(MyThreadPool myThreadPool, Func<TResult> supplier)
        {
            _threadPool = myThreadPool;
            _supplier = supplier;
        }

        /// <summary>
        /// Calculates the result
        /// </summary>
        public TResult GetResult()
        {
            lock (_locker)
            {
                if (IsCompleted) return CalculatedValue;
                try
                {
                    CalculatedValue = _supplier();
                    IsCompleted = true;
                }
                catch (Exception e)
                {
                    throw new AggregateException(e);
                }

                return CalculatedValue;
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
            if (!IsCompleted) GetResult();
            return _threadPool.Submit(() => newFunc(Result));
        }
    }

    public MyThreadPool(int threadsCount)
    {
        _queue = new ConcurrentQueue<Action>();
        _threads = new List<Thread>(threadsCount);
        _cts = new CancellationTokenSource();
        for (var i = 0; i < threadsCount; i++)
        {
            _threads.Add(new Thread(() =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_queue.IsEmpty) continue;
                    _queue.TryDequeue(out var result);
                    result?.Invoke();
                }
            }));
            _threads[i].Start();
        }
    }


    /// <summary>
    /// Adds a new task to a thread safe queue
    /// </summary>
    /// <param name="function">Input function</param>
    /// <typeparam name="TResult">Type of the result</typeparam>
    /// <returns></returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (_cts.IsCancellationRequested) throw new OperationCanceledException();
        var newTask = new MyTask<TResult>(this, function);
        _queue.Enqueue(() => { newTask.GetResult(); });
        return newTask;
    }

    /// <summary>
    /// Stops threads work with a CancellationToken
    /// </summary>
    public void Shutdown()
    {
        if (_cts.IsCancellationRequested) return;
        _cts.Cancel();
        foreach (var thread in _threads)
        {
            thread.Join();
        }
    }
}