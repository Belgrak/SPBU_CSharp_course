namespace Lazy;

public class LazyFactory
{
    public static Lazy<T> CreateSimpleLazy<T>(Func<T?> supplier)
    {
        return new Lazy<T>(supplier);
    }

    public static MultithreadingSafeLazy<T> CreateMultithreadingSafeLazy<T>(Func<T> supplier)
    {
        return new MultithreadingSafeLazy<T>(supplier);
    }
}