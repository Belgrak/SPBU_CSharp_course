using System.Collections.Concurrent;

namespace ThreadPool;

public class MyThreadPool : IDisposable
{
    private int _nThreads;
    private readonly ConcurrentQueue<Action> _queue;
    private readonly List<Thread> _threads;
    private readonly bool _isShutdown;
    private readonly CancellationTokenSource _cts;

    public MyThreadPool(int threadsCount)
    {
        _nThreads = threadsCount;
        _queue = new ConcurrentQueue<Action>();
        _threads = new List<Thread>(_nThreads);
        _isShutdown = false;
        _cts = new CancellationTokenSource();
        for (var i = 0; i < _nThreads; i++)
        {
            _threads.Add(new Thread(() =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (!_queue.IsEmpty) continue;
                    _queue.TryDequeue(out var result);
                    result?.Invoke();
                }
            }));
            _threads[i].Start();
        }
    }


    public MyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        var newTask = new MyTask<TResult>(this, function);
        _queue.Enqueue(() => newTask.CalculateResult());
        return newTask;
    }

    public void Shutdown()
    {
        if (_isShutdown) return;
        _cts.Cancel();
        foreach (var thread in _threads)
        {
            thread.Join();
        }
    }

    public void Dispose()
    {
        if (_isShutdown) return;
        Shutdown();
    }
}