using System.Collections.Generic;
using System.Numerics;

public interface IAABBTreeBase
{
    int Count { get; }
    bool IsReadonly { get; }

    RayCastHit<T> BoundingPrimativeCast<T>(IBoundingShape myTestBox);
    int CompareTo(SimpleAABBTree other);
    bool Contains(IDynamicObject myItem);
    bool Contains(IStaticObject myItem);
    bool Contains(object myItem);
    StaticAABBTree GetAreaAsStaticTree(BoundingBox Areabounds);
    IEnumerable<T> GetEnumerator<T>();
    List<T> GetNearest<T>(Vector3 position, uint Count);
    List<T> GetNearest<T>(Vector3 position, uint Count, float MaxDistance);
    List<T> GetObjectList<T>() where T : class;
    void ProcessNodeData(SimpleAABBTree.AABBDynamicDataNode internalData);
    void ProcessNodeData(AABBStaticDataNode internalData);
    RayCastHit<T> Raycast<T>(Ray aRay);
    RayCastHit<T> RaycastReturnFirst<T>(Ray aRay);
    void SetReadOnly(bool v);
}