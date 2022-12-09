using Lazy;
using NUnit.Framework;

namespace LazyTests;

public class CommonTests
{
    [Test]
    public void SimpleDataTest()
    {
        const bool value = false;
        var exampleLazy = LazyFactory.CreateSimpleLazy(() => value);
        Assert.Multiple(() =>
        {
            Assert.That(exampleLazy.IsValueCalculated, Is.EqualTo(false));
            var result = exampleLazy.Get();
            Assert.That(result, Is.EqualTo(value));
            Assert.That(exampleLazy.IsValueCalculated, Is.EqualTo(true));
        });
    }

    [Test]
    public void NullTest()
    {
        int? NullFunc()
        {
            return null;
        }

        var exampleLazy = LazyFactory.CreateSimpleLazy(NullFunc);
        Assert.Multiple(() =>
        {
            Assert.That(exampleLazy.IsValueCalculated, Is.EqualTo(false));
            var result = exampleLazy.Get();
            Assert.That(result, Is.EqualTo(null));
            Assert.That(exampleLazy.IsValueCalculated, Is.EqualTo(true));
        });
    }

    [Test]
    public void SingularityValueTest()
    {
        var random = new Random();
        var exampleLazy = LazyFactory.CreateSimpleLazy(() => random.Next(100));
        Assert.Multiple(() =>
        {
            Assert.That(exampleLazy.IsValueCalculated, Is.EqualTo(false));
            var firstCallResult = exampleLazy.Get();
            var secondCallResult = exampleLazy.Get();
            Assert.That(secondCallResult, Is.EqualTo(firstCallResult));
            Assert.That(exampleLazy.IsValueCalculated, Is.EqualTo(true));
        });
    }
}