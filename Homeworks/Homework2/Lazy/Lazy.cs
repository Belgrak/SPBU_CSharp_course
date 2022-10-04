namespace Lazy;

public class Lazy<T> : ILazy<T>
{
    private T? _object;
    private readonly Func<T?> _supplier;
    public bool IsValueCalculated { get; private set; }

    public Lazy(Func<T?> supplier)
    {
        _supplier = supplier;
    }

    public T? Get()
    {
        if (IsValueCalculated) return _object;
        _object = _supplier();
        IsValueCalculated = true;
        return _object;
    }
}