using H1Task1;
using NUnit.Framework;
namespace H1Task1Tests;

public class Tests
{
    [Test]
    [Repeat(10)]
    public void PositiveDataTest()
    {
        var random = new Random();
        var list = new List<int>();
        var n = random.Next(25);
        for (var i = 0; i < n; i++)
        {
            list.Add(random.Next(100));
        }

        var listCopy = list.GetRange(0, list.Count);
        listCopy.Sort();
        Assert.That(listCopy, Is.EqualTo(InsertionSort.Sort(list)));
    }
    
    [Test]
    [Repeat(10)]
    public void NegativeDataTest()
    {
        var random = new Random();
        var list = new List<int>();
        var n = random.Next(25);
        for (var i = 0; i < n; i++)
        {
            list.Add(random.Next(-100, 0));
        }

        var listCopy = list.GetRange(0, list.Count);
        listCopy.Sort();
        Assert.That(listCopy, Is.EqualTo(InsertionSort.Sort(list)));
    }

    [Test]
    [Repeat(10)]
    public void SortedBeforeDataTest()
    {
        var random = new Random();
        var list = new List<int>();
        var n = random.Next(25);
        for (var i = 0; i < n; i++)
        {
            list.Add(random.Next(100));
        }

        list.Sort();
        Assert.That(list, Is.EqualTo(InsertionSort.Sort(list)));
    }
}