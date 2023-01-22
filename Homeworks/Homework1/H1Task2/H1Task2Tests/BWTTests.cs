using H1Task2;
using NUnit.Framework;

namespace H1Task2Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void DirectBWTTest()
    {
        const string example = "ABACABA";
        var transformed = BurrowsWheelerTransformation.Transform(example);
        Assert.Multiple(() =>
        {
            Assert.That(transformed.Item1, Is.EqualTo("BCABAAA"));
            Assert.That(transformed.Item2, Is.EqualTo(2));
        });
    }

    [Test]
    public void ReverseBWTTest()
    {
        const string example = "BCABAAA";
        const int exampleIndex = 2;
        var reversed = BurrowsWheelerTransformation.ReverseTransform(example, exampleIndex);
        Assert.That(reversed, Is.EqualTo("ABACABA"));
    }

    [Test]
    [Repeat(10)]
    public void DirectAndReserveBWTTest()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=";
        var length = random.Next(10);
        var stringBefore = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        var transformed = BurrowsWheelerTransformation.Transform(stringBefore);
        var reversed = BurrowsWheelerTransformation.ReverseTransform(transformed.Item1, transformed.Item2);
        Assert.That(reversed, Is.EqualTo(stringBefore));
    }
}