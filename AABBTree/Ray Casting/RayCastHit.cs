
using System;
using System.Collections;
using System.Collections.Generic;

public class RayCastHit<T> : IEnumerator<T>, IEnumerable<T>, IReadOnlyCollection<T>
{
    private static T[] Empty = new T[0];
    private List<T> list;
    int Counter = -1;
    public RayCastHit(List<T> list)
    {
        this.list = list;
    }
    public RayCastHit()
    {
        list = new List<T>();
    }

    public T this[int index]
    {
        get
        {
            if (list == null) return default(T);
            return list[index];
        }
    }
    public int Count { get { return list.Count; } }

    public T Current
    {
        get
        {
            if (list != null && list.Count != 0)
                return list[Counter];
            else
                throw new Exception("Invalid Attempt at Iterating Raycast");
        }
    }

    object IEnumerator.Current
    {
        get
        {
            if (list.Count != 0)
                return list[Counter];
            else
                throw new Exception("Invalid Attempt at Iterating Raycast");
        }
    }

    public void Dispose()
    {

    }

    public bool MoveNext()
    {
        if (list == null) return false;
        Counter++;
        if (list.Count > Counter)
        {
            return true;
        }
        else
            return false;
    }

    public void Reset()
    {
        Counter = -1;
    }

    internal void MergeWith(RayCastHit<T> p)
    {
        list.AddRange(p);
    }

    public List<T> GetAsList()
    {
        if (list != null)
            return this.list;
        else
            throw new Exception("Attempted to get a list from RayCastHit<T> that was empty.");
    }

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }

    public T[] GetAsArray()
    {
        if (this.list.Count != 0)
            return this.list.ToArray();
        else
            return Empty;
    }
}