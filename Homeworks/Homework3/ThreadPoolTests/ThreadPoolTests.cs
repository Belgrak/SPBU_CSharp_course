using ThreadPool;
using NUnit.Framework;

namespace ThreadPoolTests;

public class Tests
{
    [Test]
    public void SimpleTest()
    {
        var pool = new MyThreadPool(4);
        var task = pool.Submit(() => 4);
        Assert.AreEqual(4, task.Result);
        pool.Shutdown();
    }

    [Test]
    public void ContinuationTest()
    {
        var pool = new MyThreadPool(4);
        var task = pool.Submit(() => 4);
        var continuation = task.ContinueWith((result) => result * 2);
        Assert.Multiple(() =>
        {
            Assert.AreEqual(4, task.Result);
            Assert.AreEqual(8, continuation.Result);
        });
        pool.Shutdown();
    }

    [Test]
    public void SingleCalculationTest()
    {
        var pool = new MyThreadPool(4);
        var random = new Random();
        var task = pool.Submit(() => random.Next());
        Assert.AreEqual(task.Result, task.Result);
        pool.Shutdown();
    }

    [Test]
    public void ShutdownTest()
    {
        var pool = new MyThreadPool(4);
        var task = pool.Submit(() => 2 * 3);
        pool.Shutdown();
        Assert.Throws<OperationCanceledException>(() => pool.Submit(() => 2 * 3));
    }

    [Test]
    public void MultipleTasksTest()
    {
        var pool = new MyThreadPool(5);

        var listOfTasks = new List<IMyTask<int>>();
        for (var i = 0; i < 100; i++)
        {
            var index = i;
            listOfTasks.Add(pool.Submit(() => index));
        }
        Assert.Multiple(() =>
        {
            for (var i = 0; i < 99; i++)
            {
                var index = i;
                listOfTasks.Add(pool.Submit((() => index)));
                Assert.AreEqual(i, listOfTasks[i].Result);
            }
        });
        pool.Shutdown();
    }
}