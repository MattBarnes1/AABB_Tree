using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class AABBTreeBase : IComparable<SimpleAABBTree>, IAABBTreeVistor, IAABBTreeBase
{

    public IEnumerable<T> GetEnumerator<T>()
    {
        return new AABBObjectListIterator<T>(this, DynamicNodeLookup, this.StaticNodeLookup);
    }

        

    protected Dictionary<Object, IAABBNode> StaticNodeLookup = new Dictionary<Object, IAABBNode>();
    protected Dictionary<Object, IAABBNode> DynamicNodeLookup = new Dictionary<Object, IAABBNode>();
    protected IAABBNode Root;
    public int Count { get { return DynamicNodeLookup.Count + StaticNodeLookup.Count; } }

    protected AABBTreeBase() { }
    protected AABBTreeBase(IAABBNode RootNode)
    {
        this.Root = RootNode;
    }

    bool RebuildDynamicOnly;
    protected void ReinitializeTreeFromRootItem(bool DynamicOnly = false)
    {
        if (IsReadonly) throw new Exception("Someone was either stuck processing or didn't properly dispose of Enumerator for AABBTree.");
        RebuildDynamicOnly = DynamicOnly;
        if (DynamicOnly)
        {
            foreach (var A in DynamicNodeLookup)
            {
                RemoveNode(A.Value);
            }
            DynamicNodeLookup.Clear();
        }
        IAABBNode Current = Root;
        Queue<IAABBNode> NextToExamine = new Queue<IAABBNode>();
        while (Current != null)
        {
            Current.Visit(this);
            if (Current.Left != null)
                NextToExamine.Enqueue(Current.Left);
            if (Current.Right != null)
                NextToExamine.Enqueue(Current.Right);
            if (NextToExamine.Count != 0)
                Current = NextToExamine.Dequeue();
            else
                break;
        }
    }



    private int RemoveInvalidParents(ref IAABBNode NodeParent, out IAABBNode lastModifiedNode)
    {

        int Count = 0;

        lastModifiedNode = NodeParent;
        while (!CheckValidNode(NodeParent) && NodeParent.Parent != null)
        {
            lastModifiedNode = NodeParent;
            NodeParent = NodeParent.Parent;
            Count++;
        }


        return Count;
    }

    public T GetObjectDataContainingPoint<T>(Vector3 Point)
    {
        Stack<IAABBNode> NodesToExplore = new Stack<IAABBNode>();
        IAABBNode Current = Root;
        if (Current.ContainsData())
        {
            if (Current.Bounds.Contains(Point) == ContainmentType.Contains)
            {
               return (T)((IAABBTreeDataHolder)Current).GetInternalData();
            }
        }
        while (Current != null)
        {
            if (Current.Bounds.Contains(Point) == ContainmentType.Contains)//So it hit.
            {
                if (Current.Left.ContainsData() && Current.Left is IAABBTreeDataHolder)
                {
                    if(Current.Left.Bounds.Center == Point)
                    {
                        return (T)((IAABBTreeDataHolder)Current.Left).GetInternalData();
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    NodesToExplore.Push(Current.Left);
                }
            }
            if (Current.Bounds.Contains(Point) == ContainmentType.Contains) //So it hit.
            {
                if (Current.Right.ContainsData() && Current.Right is IAABBTreeDataHolder)
                {
                    if (Current.Right.Bounds.Center == Point)
                    {
                        return (T)((IAABBTreeDataHolder)Current.Right).GetInternalData();
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    NodesToExplore.Push(Current.Right);
                }
            }
            if (NodesToExplore.Count == 0)
            {
                break;
            }
            Current = NodesToExplore.Pop();
        }
        return  default(T);
    }

    public List<T> GetNearest<T>(Vector3 position, uint Count, float MaxDistance)
    {
        if (Count == 0) throw new Exception("Attempted to perform get nearest search with a return count set to 0");
        if (Root == null) return new List<T>();
        List<T> myResults = new List<T>((int)Count);
        Stack<IAABBNode> NodesToExplore = new Stack<IAABBNode>();
        Stack<T> MostClosestItem = new Stack<T>();

        IAABBNode Current = Root;
        if (Current == null) return new List<T>();
        if (Current.ContainsData())
        {
            if (IsWithinRangeOfPosition(position, MaxDistance, Current.Bounds))
            {
                TryDataConvert(myResults, Current);
            }
            else return null;
        }
        while (Current != null)
        {
            if (Current.Left != null && (Current.Right == null || !IsWithinRangeOfPosition(position, MaxDistance, Current.Right.Bounds)))
            {
                if (IsWithinRangeOfPosition(position, MaxDistance, Current.Left.Bounds))
                {
                    if ((Current.Left.ContainsData())) //this only happens if we're at the bottom of the tree.
                    {
                        TryDataConvert<T>(MostClosestItem, Current.Left);
                    }
                    else
                    {
                        NodesToExplore.Push(Current.Left);
                    }
                }
            }
            else if (Current.Right != null && (Current.Left == null || !IsWithinRangeOfPosition(position, MaxDistance, Current.Left.Bounds)))
            {
                if (IsWithinRangeOfPosition(position, MaxDistance, Current.Right.Bounds))
                {
                    if ((Current.Right.ContainsData())) //this only happens if we're at the bottom of the tree.
                    {
                        TryDataConvert<T>(MostClosestItem, Current.Right);
                    }
                    else
                    {
                        NodesToExplore.Push(Current.Right);
                    }
                }
            }
            else if (Current.Left != null && Current.Right != null)
            {
                if (Current.Left.ContainsData() && Current.Right.ContainsData())
                {
                    TryDataConvert<T>(MostClosestItem, Current.Left);
                    TryDataConvert<T>(MostClosestItem, Current.Right);
                }
                else if (Current.Left.ContainsData())
                {
                    TryDataConvert<T>(MostClosestItem, Current.Left);
                    NodesToExplore.Push(Current.Right);
                }
                else if (Current.Right.ContainsData())
                {
                    TryDataConvert<T>(MostClosestItem, Current.Right);
                    NodesToExplore.Push(Current.Left);
                }
                else
                { //pushes nearest node into the stack first so it pop first.
                    if (Vector3.Distance(Current.Right.Bounds.Center, position) > Vector3.Distance(Current.Left.Bounds.Center, position))
                    {
                        NodesToExplore.Push(Current.Right);
                        NodesToExplore.Push(Current.Left);
                    }
                    else
                    {
                        NodesToExplore.Push(Current.Left);
                        NodesToExplore.Push(Current.Right);
                    }

                }
            }
            if (NodesToExplore.Count == 0 || (MostClosestItem.Count == Count))
            {
                break;
            }
            Current = NodesToExplore.Pop();
        }
        if (Count > 0)
        {
            int Counter = (int)Math.Min(Count, MostClosestItem.Count);
            for (int i = 0; i < Counter; i++)
            {
                myResults.Add(MostClosestItem.Pop());
            }
        }
        else
        {
            int Counter = MostClosestItem.Count;
            for (int i = 0; i < Counter; i++)
            {
                myResults.Add(MostClosestItem.Pop());
            }
        }
        return myResults;

    }

    protected void RemoveNode(IAABBNode nodeData)
    {
        IAABBNode LastModified;
        if (nodeData != Root)
        {
            if (nodeData.Parent.Left == nodeData)
            {
                nodeData.Parent.Left = null;
            }
            else if (nodeData.Parent.Right == nodeData)
            {
                nodeData.Parent.Right = null;
            }
            var Parent = nodeData.Parent;
            RemoveInvalidParents(ref Parent, out LastModified);
            if (LastModified != Root && !CheckValidNode(LastModified))
                throw new Exception("AABBTree Node became invalid after adding and remove.");
            else if (LastModified == Root && !CheckValidNode(LastModified))
            {
                Root = null;
            }
        }
        else
        {
            RemoveInvalidParents(ref Root, out LastModified);
            if (Root != null && !CheckValidNode(Root))
                throw new Exception("AABBTree Node became invalid after adding and remove.");
        }
    }

    private bool CheckValidNode(IAABBNode lastModified)
    {
        if (lastModified.ContainsData())
        {
            if (lastModified.Left != null || lastModified.Right != null) return false;
        }
        else
        {
            if (lastModified.Left == null && lastModified.Right == null) return false;
        }
        return true;
    }

    private void TryDataConvert<T>(Stack<T> mostClosestItem, IAABBNode Current)
    {
        var Test = (T)((IAABBTreeDataHolder)Current).GetInternalData();
        mostClosestItem.Push(Test);
    }

    private static void TryDataConvert<T>(List<T> myResults, IAABBNode Current)
    {
        var Test = (T)((IAABBTreeDataHolder)Current).GetInternalData();
        myResults.Add(Test);
    }

    private static bool IsWithinRangeOfPosition(Vector3 position, float MaxDistance, BoundingBox myBox)
    {
        return MaxDistance == -1 || (Vector3.Distance(myBox.Center, position) <= MaxDistance);
    }
    public List<T> GetNearest<T>(Vector3 position, uint ReturnCount)
    {
        return GetNearest<T>(position, ReturnCount, -1);
    }
    public RayCastHit<T> Raycast<T>(Ray aRay)
    {
        if (Root == null) return new RayCastHit<T>(new List<T>());
        List<T> myResults = new List<T>();
        Stack<IAABBNode> NodesToExplore = new Stack<IAABBNode>();
        IAABBNode Current = Root;
        if (Current.ContainsData())
        {
            if (aRay.Intersects(Current.Bounds).HasValue)
            {
                TryDataConvert<T>(myResults, Current);
            }
        }
        while (Current != null)
        {
            if (aRay.Intersects(Current.Left.Bounds).HasValue) //So it hit.
            {
                if (Current.Left.ContainsData() && Current.Left is IAABBTreeDataHolder)
                {
                    TryDataConvert<T>(myResults, Current.Left);
                }
                else
                {
                    NodesToExplore.Push(Current.Left);
                }
            }
            if (aRay.Intersects(Current.Right.Bounds).HasValue) //So it hit.
            {
                if (Current.Right.ContainsData() && Current.Right is IAABBTreeDataHolder)
                {
                    TryDataConvert<T>(myResults, Current.Right);
                }
                else
                {
                    NodesToExplore.Push(Current.Right);
                }
            }
            if (NodesToExplore.Count == 0)
            {
                break;
            }
            Current = NodesToExplore.Pop();
        }
        return new RayCastHit<T>(myResults);
    }
    public RayCastHit<T> RaycastReturnFirst<T>(Ray aRay)
    {
        if (Root == null) return new RayCastHit<T>(new List<T>());
        List<T> myResults = new List<T>();
        Stack<IAABBNode> NodesToExplore = new Stack<IAABBNode>();
        IAABBNode Current = Root;
        if (Current.ContainsData())
        {
            if (aRay.Intersects(Current.Bounds).HasValue)
            {
                TryDataConvert<T>(myResults, Current);
                return new RayCastHit<T>(myResults);
            }
        }
        while (Current != null)
        {
            if (aRay.Intersects(Current.Left.Bounds).HasValue) //So it hit.
            {
                if (Current.Left.ContainsData() && Current.Left is IAABBTreeDataHolder)
                {
                    TryDataConvert<T>(myResults, Current.Left);
                    break;
                }
                else
                {
                    NodesToExplore.Push(Current.Left);
                }
            }
            if (aRay.Intersects(Current.Right.Bounds).HasValue) //So it hit.
            {
                if (Current.Right.ContainsData() && Current.Right is IAABBTreeDataHolder)
                {
                    TryDataConvert<T>(myResults, Current.Right);
                    break;
                }
                else
                {
                    NodesToExplore.Push(Current.Right);
                }
            }
            if (NodesToExplore.Count == 0)
            {
                break;
            }
            Current = NodesToExplore.Pop();
        }
        return new RayCastHit<T>(myResults);
    }

    public delegate bool DataPredicateTest(IAABBNode myTest);


    private IEnumerable<IAABBNode> IterateDataNodes(DataPredicateTest myTestPredicate, IAABBNode Current)
    {
        Stack<IAABBNode> NodesToExplore = new Stack<IAABBNode>();
        if (Current.ContainsData())
        {
            if (myTestPredicate(Current))
            {
                yield return Current;
            }
            yield break;
        }
        while (Current != null)
        {
            if (myTestPredicate(Current.Left)) //So it hit.
            {
                if (Current.Left.ContainsData() && Current.Left is IAABBTreeDataHolder)
                {
                    yield return Current.Left;
                }
                else
                {
                    NodesToExplore.Push(Current.Left);
                }
            }
            if (myTestPredicate(Current.Right)) //So it hit.
            {
                if (Current.Right.ContainsData() && Current.Right is IAABBTreeDataHolder)
                {
                    yield return Current.Right;
                }
                else
                {
                    NodesToExplore.Push(Current.Right);
                }
            }
            if (NodesToExplore.Count == 0)
            {
                break;
            }
            Current = NodesToExplore.Pop();
        }
    }

   

    public RayCastHit<T> BoundingPrimativeCast<T>(IBoundingShape myTestBox)
    {
        if (Root == null) return new RayCastHit<T>(new List<T>());
        List<T> myResults = new List<T>();
        Stack<IAABBNode> NodesToExplore = new Stack<IAABBNode>();
        IAABBNode Current = Root;
        if (Current.ContainsData())
        {
            if(Current.Bounds.Contains(myTestBox) != ContainmentType.Disjoint)
            {
                TryDataConvert<T>(myResults, Current);
            }
            return new RayCastHit<T>(myResults);
        }
        while (Current != null)
        {
            ContainmentType left = ContainmentType.Disjoint;
            ContainmentType right = ContainmentType.Disjoint;
            if (Current.Left != null)
                left = myTestBox.Contains(Current.Left.Bounds);
            if (Current.Right != null)
                right = myTestBox.Contains(Current.Right.Bounds);
            if (left == ContainmentType.Contains || left == ContainmentType.Intersects) //So it hit.
            {
                if (Current.Left.ContainsData())
                {
                    TryDataConvert<T>(myResults, Current.Left);
                }
                else
                {
                    NodesToExplore.Push(Current.Left);
                }
            }
            if (right == ContainmentType.Contains || right == ContainmentType.Intersects) //So it hit.
            {
                if (Current.Right.ContainsData())
                {
                    TryDataConvert<T>(myResults, Current.Right);
                }
                else
                {
                    NodesToExplore.Push(Current.Right);
                }
            }
            if (NodesToExplore.Count == 0)
            {
                break;
            }
            Current = NodesToExplore.Pop();
        }
        return new RayCastHit<T>(myResults);
    }

    public List<T> GetObjectList<T>() where T : class
    {
        List<T> myObjects = new List<T>((int)Math.Ceiling(DynamicNodeLookup.Count * 0.5f));
        foreach (var A in DynamicNodeLookup)
        {
            if (A.Key is T)
                myObjects.Add(A.Key as T);
        }
        foreach (var A in StaticNodeLookup)
        {
            if (A.Key is T)
                myObjects.Add(A.Key as T);
        }
        return myObjects;
    }
    public int CompareTo(SimpleAABBTree other)
    {
        Dictionary<Object, IAABBNode> LargerStaticNodeLookupDictionary;
        Dictionary<object, IAABBNode> SmallerStaticNodeLookupDictionary;
        Dictionary<Object, IAABBNode> LargerDynamicNodeLookupDictionary;
        Dictionary<Object, IAABBNode> SmallerDynamicNodeLookupDictionary;
        int difference = 0;

        if (this.DynamicNodeLookup.Count < other.DynamicNodeLookup.Count)
        {
            LargerDynamicNodeLookupDictionary = other.DynamicNodeLookup;
            SmallerDynamicNodeLookupDictionary = this.DynamicNodeLookup;
            difference = CompareKeys(LargerDynamicNodeLookupDictionary, SmallerDynamicNodeLookupDictionary);
        }
        else
        {
            LargerDynamicNodeLookupDictionary = this.DynamicNodeLookup;
            SmallerDynamicNodeLookupDictionary = other.DynamicNodeLookup;
            difference = -CompareKeys(LargerDynamicNodeLookupDictionary, SmallerDynamicNodeLookupDictionary);
        }
        if (difference != 0) return difference;
        if (this.StaticNodeLookup.Count < other.StaticNodeLookup.Count)
        {
            LargerStaticNodeLookupDictionary = other.StaticNodeLookup;
            SmallerStaticNodeLookupDictionary = this.StaticNodeLookup;
            difference = CompareKeys(LargerStaticNodeLookupDictionary, SmallerStaticNodeLookupDictionary);
        }
        else
        {
            LargerStaticNodeLookupDictionary = this.StaticNodeLookup;
            SmallerStaticNodeLookupDictionary = other.StaticNodeLookup;
            difference = -CompareKeys(LargerStaticNodeLookupDictionary, SmallerStaticNodeLookupDictionary);
        }
        return difference;
    }

    private int CompareKeys(Dictionary<object, IAABBNode> largerStaticNodeLookupDictionary, Dictionary<object, IAABBNode> smallerStaticNodeLookupDictionary)
    {
        foreach (var A in largerStaticNodeLookupDictionary)
        {
            if (!smallerStaticNodeLookupDictionary.ContainsKey(A.Key))
            {
                return 1;
            }
        }
        return 0;

    }

    /// <summary>
    /// This currently runs at an O(1) complexity but more memory usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="myItem"></param>
    /// <returns></returns>
    public bool Contains(object myItem)
    {
        return DynamicNodeLookup.ContainsKey((object)myItem) || StaticNodeLookup.ContainsKey((object)myItem);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="myItem"></param>
    /// <returns></returns>
    public bool ContainsPoint(System.Numerics.Vector3 myItem)
    {
        foreach(var A in this.IterateDataNodes(delegate (IAABBNode myNode) { return (myNode.Bounds.Contains(myItem) == ContainmentType.Contains); }, Root))
        {
            return true;
        }
        return false;
    }



    /// <summary>
    /// This currently runs at an O(1) complexity but more memory usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="myItem"></param>
    /// <returns></returns>
    public bool Contains(IDynamicObject myItem)
    {
        return DynamicNodeLookup.ContainsKey((object)myItem);
    }
    /// <summary>
    /// This currently runs at an O(1) complexity but more memory usage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="myItem"></param>
    /// <returns></returns>
    public bool Contains(IStaticObject myItem)
    {
        return StaticNodeLookup.ContainsKey((object)myItem);
    }

    public StaticAABBTree GetAreaAsStaticTree(BoundingBox Areabounds)
    {
        var Result = GetMinimalBranch(Areabounds);
        return new StaticAABBTree(Result, Areabounds);
    }


    protected IAABBNode GetMinimalBranch(IBoundingShape areabounds)
    {
        if (Root == null) return null;
        IAABBNode CurrentMinimalObject = Root;
        Stack<IAABBNode> LastChecked = new Stack<IAABBNode>();
        while (CurrentMinimalObject != null)
        {
            ContainmentType leftContainment = ContainmentType.Disjoint;
            ContainmentType rightContainment = ContainmentType.Disjoint;
            if (CurrentMinimalObject.Left != null)
                leftContainment = CurrentMinimalObject.Left.Bounds.Contains(areabounds);
            if (CurrentMinimalObject.Right != null)
                rightContainment = CurrentMinimalObject.Right.Bounds.Contains(areabounds);
            else if (CurrentMinimalObject.Left == null)
                break;

            if (leftContainment == ContainmentType.Contains || (leftContainment == ContainmentType.Intersects && rightContainment == ContainmentType.Disjoint))
            {
                CurrentMinimalObject = CurrentMinimalObject.Left;
            }
            else if (rightContainment == ContainmentType.Contains || (rightContainment == ContainmentType.Intersects && leftContainment == ContainmentType.Disjoint))
            {
                CurrentMinimalObject = CurrentMinimalObject.Right;
            }
            else if (leftContainment == ContainmentType.Intersects && rightContainment == ContainmentType.Intersects)
            {
                break;
            }
            else
                throw new Exception("Tree was left in unclean state.");
        }
        return CurrentMinimalObject;
    }

    object Lock = new object();
    int Readonly = 0;
    public bool IsReadonly
    {
        get
        {
            lock(Lock)
            {
                return Readonly != 0;

            }
        }
    }

    public void SetReadOnly(bool v)
    {
        //if (Lock == null) throw new Exception("Lock Nulled");
        lock (Lock)
        {
            if (v)
            {
                Readonly++;
            }
            else
            {
                Readonly--;
            }
        }
    }





    public virtual void ProcessNodeData(AABBStaticDataNode internalData)
    {
        if (!RebuildDynamicOnly)
            StaticNodeLookup.Add(internalData.Data, internalData);
    }

    public virtual void ProcessNodeData(SimpleAABBTree.AABBDynamicDataNode internalData)
    {
        DynamicNodeLookup.Add(internalData.GetInternalData(), internalData);
    }

}