using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class MergedAABBTree
{
    List<AABBTreeBase> myViewedTree = new List<AABBTreeBase>();

    public int Count { get; private set; }

    public bool IsReadonly { get; private set; }

    internal void Merge(StaticAABBTree staticAABBTree)
    {
        myViewedTree.Add(staticAABBTree);
    }

    internal void Clear()
    {
        myViewedTree.Clear();
    }

    public RayCastHit<T> BoundingPrimativeCast<T>(BoundingBox myTestBox) //TODO: parallel
    {
        RayCastHit<T> myRayCast = new RayCastHit<T>();
        foreach(var A in myViewedTree)
        {
            myRayCast.MergeWith(A.BoundingPrimativeCast<T>(myTestBox));
        }
        return myRayCast;
    }

    public int CompareTo(SimpleAABBTree other)
    {
        throw new NotImplementedException();
    }

    public bool Contains(IDynamicObject myItem)
    {
        foreach (var A in myViewedTree)
        {
            if (A.Contains(myItem)) return true;
        }
        return false;
    }

    public bool Contains(IStaticObject myItem)
    {
        foreach (var A in myViewedTree)
        {
            if (A.Contains(myItem)) return true;
        }
        return false;
    }

    public bool Contains(object myItem)
    {
        foreach (var A in myViewedTree)
        {
            if (A.Contains(myItem)) return true;
        }
        return false;
    }


    public IEnumerable<T> GetEnumerator<T>()
    {
        return new MergedTreeIterator<T>(this.myViewedTree);
    }

    public List<T> GetNearest<T>(Vector3 position, uint Count)
    {
        List<T> ReturnList = new List<T>();
        int CountPerRegion = (int)Math.Floor((float)(Count / this.myViewedTree.Count));
        foreach (var A in myViewedTree)
        {
            ReturnList.AddRange(A.GetNearest<T>(position, (uint)CountPerRegion));
        }
        return ReturnList;
    }

    public List<T> GetNearest<T>(Vector3 position, uint Count, float MaxDistance)
    {
        List<T> ReturnList = new List<T>();
        int CountPerRegion = (int)Math.Floor((float)(Count / this.myViewedTree.Count));
        foreach (var A in myViewedTree)
        {
            ReturnList.AddRange(A.GetNearest<T>(position, (uint)CountPerRegion, MaxDistance));
        }
        return ReturnList;
    }

    public List<T> GetObjectList<T>() where T : class
    {
        List<T> ReturnList = new List<T>();
        int CountPerRegion = (int)Math.Floor((float)(Count / this.myViewedTree.Count));
        foreach (var A in myViewedTree)
        {
            ReturnList.AddRange(A.GetObjectList<T>());
        }
        return ReturnList;
    }


    public RayCastHit<T> Raycast<T>(Ray aRay)
    {
        RayCastHit<T> myRayCast = new RayCastHit<T>();
        foreach (var A in myViewedTree)
        {
            myRayCast.MergeWith(A.Raycast<T>(aRay));
        }
        return myRayCast;
    }

    public RayCastHit<T> RaycastReturnFirst<T>(Ray aRay)
    {
        RayCastHit<T> myRayCast = new RayCastHit<T>();
        foreach (var A in myViewedTree)
        {
            var Result = A.RaycastReturnFirst<T>(aRay);
            if (Result.Count != 0)
            {
                myRayCast = Result;
                return myRayCast;
            }
        }
        return myRayCast;
    }
}