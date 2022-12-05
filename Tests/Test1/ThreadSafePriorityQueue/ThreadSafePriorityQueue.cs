namespace ThreadSafePriorityQueue;

/// <summary>Thread-safe priority queue.</summary>
/// <typeparam name="TV">The type of elements.</typeparam>
/// <typeparam name="TP">The type of priority. Inheritance from <see cref="IComparable"/> required.</typeparam>
public class ThreadSafePriorityQueue<TV, TP>
    where TP : IComparable
    where TV : struct
{
    private readonly SortedDictionary<TP, Queue<TV>> _queueMap = new();
    private int _elementsCount;

    /// <summary>Returns elements count in queue.</summary>
    public int Size
    {
        get
        {
            lock (_queueMap)
            {
                return _elementsCount;
            }
        }
    }

    /// <summary>Add element with priority to queue.</summary>
    /// <param name="value">Element's value.</param>
    /// <param name="priority">Element's priority.</param>
    public void Enqueue(TV value, TP priority)
    {
        lock (_queueMap)
        {
            if (!_queueMap.ContainsKey(priority))
            {
                _queueMap.Add(priority, new Queue<TV>());
            }

            _queueMap[priority]?.Enqueue(value);
            _elementsCount++;
            Monitor.PulseAll(_queueMap);
        }
    }

    /// <summary>Returns element with maximum priority from queue.</summary>
    public TV? Dequeue()
    {
        lock (_queueMap)
        {
            while (_elementsCount == 0)
            {
                Monitor.Wait(_queueMap);
            }

            var result = _queueMap[_queueMap.Keys.Last()].Dequeue();
            _elementsCount--;
            if (_queueMap[_queueMap.Keys.Last()].Count == 0)
            {
                _queueMap.Remove(_queueMap.Keys.Last());
            }

            Monitor.PulseAll(_queueMap);
            return result;
        }
    }
}