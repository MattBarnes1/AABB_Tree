using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class StaticAABBTree : AABBTreeBase
{
    BoundingBox areabounds;
    bool RebuildStaticObjects = false;
    public StaticAABBTree(IAABBNode RootNode, BoundingBox areabounds, Predicate<object> shouldEliminateItemCheck = null) : base(RootNode)
    {
        this.areabounds = areabounds;
        RebuildStaticObjects = true;
        ReinitializeTreeFromRootItem();
    }

    public void RebuildStaticTreeForArea()
    {
        ReinitializeTreeFromRootItem(true);
    }

    public override void ProcessNodeData(AABBStaticDataNode internalData)
    {
        if(areabounds.Contains(internalData.Bounds) != ContainmentType.Disjoint && RebuildStaticObjects)
        {
            base.ProcessNodeData(internalData);
        }
    }

    public override void ProcessNodeData(SimpleAABBTree.AABBDynamicDataNode internalData)
    {
        if (areabounds.Contains(internalData.Bounds) != ContainmentType.Disjoint)
        {
            base.ProcessNodeData(internalData);
        }
    }


}
