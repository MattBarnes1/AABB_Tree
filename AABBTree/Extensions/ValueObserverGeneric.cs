using System;

[Serializable]
public class ValueObserver<T>
{

    public T Value
    {
        get
        {
            return aValue;
        }
        set
        {
            Set(value);
        }
    }
    T aValue;

    Action<T> SingleValueWatcher;



    Action<T, T> OldNewValueWatcher;

    Action<object> ChangedObjectWatcher;
    object aCreator;
    public ValueObserver(T myAmount, object myCreator = null)
    {
        aCreator = myCreator;
        aValue = myAmount;
    }

    public ValueObserver(object myCreator = null)
    {
        aCreator = myCreator;
        aValue = default(T);
    }


    public static bool operator ==(ValueObserver<T> anObserver, T myTime)
    {
        if (anObserver is null && myTime == null) return true;
        if ((anObserver != null && myTime == null) || (anObserver == null && myTime != null)) return false;
        return anObserver.aValue.Equals(myTime);
    }

    public static bool operator !=(ValueObserver<T> anObserver, T myTime)
    {
        if (anObserver is null && myTime == null) return false;
        if ((anObserver != null && myTime == null) || (anObserver == null && myTime != null)) return true;
        return !anObserver.aValue.Equals(myTime);
    }

    public void AddObjectObserver(Action<object> OldValueNewValueAction)
    {
        if (OldValueNewValueAction == null) throw new ArgumentNullException(OldValueNewValueAction + " was set to null!");
        if (aCreator == null) throw new Exception("Value observer requested to add an observer on the owning object, but owning object wasn't set.");
        Count++;
        OldValueNewValueAction?.Invoke(aCreator);
        ChangedObjectWatcher += OldValueNewValueAction;
    }

    public void RemoveObjectObserver(Action<object> OldValueNewValueAction)
    {
        if (OldValueNewValueAction == null) throw new ArgumentNullException(nameof(OldValueNewValueAction));
        Count--;
        ChangedObjectWatcher -= OldValueNewValueAction;
    }

    public void AddObserver(Action<T, T> OldValueNewValueAction)
    {
        if (OldValueNewValueAction == null) throw new ArgumentNullException(nameof(OldValueNewValueAction));
        Count++;
        OldValueNewValueAction?.Invoke(default(T), aValue);
        OldNewValueWatcher += OldValueNewValueAction;
    }

    public void RemoveObserver(Action<T, T> OldValueNewValueAction)
    {
        if (OldValueNewValueAction == null) throw new ArgumentNullException(nameof(OldValueNewValueAction));
        Count--;
        OldNewValueWatcher -= OldValueNewValueAction;
    }

    public T Get()
    {
        return aValue;
    }

    public T Set(T newValue)
    {
        if (newValue != null && newValue.Equals(aValue)) return aValue;
        T oldValue = aValue;
        aValue = newValue;

        OldNewValueWatcher?.Invoke(oldValue, newValue);
        SingleValueWatcher?.Invoke(newValue);
        ChangedObjectWatcher?.Invoke(aCreator);
        return Value;
    }

    public int Count { get; private set; }

    public void AddObserver(Action<T> Observer)
    {
        if (Observer == null) throw new ArgumentNullException(nameof(Observer));
        Count++;
        Observer(aValue);
        this.SingleValueWatcher += Observer;
    }

    public void RemoveObserver(Action<T> Observer)
    {
        if (Observer == null) throw new ArgumentNullException(nameof(Observer));
        Count--;
        this.SingleValueWatcher -= Observer;
    }

 
}

