namespace Lazy;

public class LazyFactory
{
    /// <summary>
    /// Creates and returns <see cref="Lazy{T}"/>
    /// </summary>
    public static Lazy<T> CreateSimpleLazy<T>(Func<T?> supplier)
    {
        return new Lazy<T>(supplier);
    }

    /// <summary>
    /// Creates and returns <see cref="MultithreadingSafeLazy{T}"/>
    /// </summary>
    public static MultithreadingSafeLazy<T> CreateMultithreadingSafeLazy<T>(Func<T> supplier)
    {
        return new MultithreadingSafeLazy<T>(supplier);
    }
}