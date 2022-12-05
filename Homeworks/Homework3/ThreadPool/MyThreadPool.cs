﻿using System.Collections.Concurrent;

namespace ThreadPool;

public class MyThreadPool
{
    private readonly ConcurrentQueue<Action> _queue;
    private readonly List<Thread> _threads;
    private readonly bool _isShutdown;
    private readonly CancellationTokenSource _cts;

    public MyThreadPool(int threadsCount)
    {
        var nThreads = threadsCount;
        _queue = new ConcurrentQueue<Action>();
        _threads = new List<Thread>(nThreads);
        _isShutdown = false;
        _cts = new CancellationTokenSource();
        for (var i = 0; i < nThreads; i++)
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
}