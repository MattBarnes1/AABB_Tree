using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MergedTreeIterator<T> : IEnumerator<T>, IEnumerable<T>
{
    public T Current { get { return CurrentTreeEnumerator.Current; } }
    object SelectedItem;
    private List<AABBTreeBase> MyIteratedTrees;

    object IEnumerator.Current { get { return CurrentTreeEnumerator.Current; } }


    public MergedTreeIterator(List<AABBTreeBase> myTreesToUse)
    {
        this.MyIteratedTrees = myTreesToUse;
    }

    public void Dispose()
    {

    }

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }

    int Counter = 0;
    IEnumerator<T> CurrentTreeEnumerator;
    public bool MoveNext()
    {
        if (CurrentTreeEnumerator == null)
            CurrentTreeEnumerator = MyIteratedTrees[0].GetEnumerator<T>().GetEnumerator();
        if (!CurrentTreeEnumerator.MoveNext())
        {
            Counter++;
            if (Counter < MyIteratedTrees.Count)
            {
                CurrentTreeEnumerator = MyIteratedTrees[Counter].GetEnumerator<T>().GetEnumerator();
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void Reset()
    {
        Counter = 0;
        CurrentTreeEnumerator = null;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }
}