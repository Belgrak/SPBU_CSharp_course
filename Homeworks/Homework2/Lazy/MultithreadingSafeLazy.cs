namespace Lazy;

/// <summary>
/// Represents Lazy, which will be safe in multithreading 
/// </summary>
public class MultithreadingSafeLazy<T> : ILazy<T>
{
    private T? _object;
    private readonly object _locker = new();
    public bool IsValueCalculated { get; private set; }
    private readonly Func<T> _supplier;

    public MultithreadingSafeLazy(Func<T> supplier)
    {
        _supplier = supplier;
    }

    
    /// <returns>First call value of <see cref="_supplier"/></returns>
    public T? Get()
    {
        if (IsValueCalculated) return _object;
        lock (_locker)
        {
            if (IsValueCalculated) return _object;
            _object = _supplier();
            IsValueCalculated = true;
            return _object;
        }
    }
}