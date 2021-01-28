using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AABBObjectListIterator<T> : IEnumerator<T>, IEnumerable<T>
{
    public AABBObjectListIterator(AABBTreeBase myTreeToLockChangesOn, Dictionary<object, IAABBNode> myNodeObjects, Dictionary<object, IAABBNode> staticNodeLookup)
    {
        this.LockedTree = myTreeToLockChangesOn;
        LockedTree.SetReadOnly(true);
        this.DynamicPartActivePair = myNodeObjects.GetEnumerator();
        this.StaticPartActivePair = staticNodeLookup.GetEnumerator();
    }

    Dictionary<object, IAABBNode> ActivelySearched;
    public T Current
    {
        get
        {
            return (T)SelectedItem;
        }
    }

    object IEnumerator.Current
    {
        get
        {
            return SelectedItem;
        }
    }

    AABBTreeBase LockedTree { get; set; }

    IEnumerator<KeyValuePair<object, IAABBNode>> DynamicPartActivePair;
    IEnumerator<KeyValuePair<object, IAABBNode>> StaticPartActivePair;
    object SelectedItem;

    public bool MoveNext()
    {
        SelectedItem = null;
        if (DynamicPartActivePair != null)
        {
            while (DynamicPartActivePair.MoveNext())
            {
                if (DynamicPartActivePair.Current.Key is T)
                {
                    SelectedItem = DynamicPartActivePair.Current.Key;
                    return true;
                }
            }
        }
        if(StaticPartActivePair != null)
        {
            while (StaticPartActivePair.MoveNext())
            {
                if (StaticPartActivePair.Current.Key is T)
                {
                    SelectedItem = StaticPartActivePair.Current.Key;
                    return true;
                }
            }
        }
        return false;
    }

    public void Reset()
    {
        DynamicPartActivePair.Reset();
        StaticPartActivePair.Reset();
    }

    public void Dispose()
    {
        LockedTree.SetReadOnly(false);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }
}