using Lazy;
using NUnit.Framework;

namespace LazyTests;

public class MultithreadingSafeLazyTests
{
    [Test]
    public async Task RaceTest()
    {
        var random = new Random();
        var exampleLazy = LazyFactory.CreateMultithreadingSafeLazy(() => random.Next(100));

        var task1 = new Task<int>(() =>
        {
            Thread.Sleep(100);
            return exampleLazy.Get();
        });
        var task2 = new Task<int>(() => exampleLazy.Get());
        var task3 = new Task<int>(() =>
        {
            var result = exampleLazy.Get();
            Thread.Sleep(100);
            return result;
        });
        task1.Start();
        task2.Start();
        task3.Start();
        var result3 = await task1;
        var result1 = await task2;
        var result2 = await task3;
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(result2));
            Assert.That(result2, Is.EqualTo(result3));
            Assert.That(exampleLazy.IsValueCalculated, Is.EqualTo(true));
        });
    }
}