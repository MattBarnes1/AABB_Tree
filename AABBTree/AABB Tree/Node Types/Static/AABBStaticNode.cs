using System;
[Serializable]
public class AABBStaticNode : IAABBNode
{
    public BoundingBox Bounds { get; set; }
    public IAABBNode Parent { get; set; }
    public IAABBNode Left { get; set; }
    public IAABBNode Right { get; set; }
    public int DepthOffset { get; set; }
    public SimpleAABBTree Owner { get; set; }

   

    public virtual bool ContainsData() { return false; }

    public void Visit(IAABBTreeVistor T)
    {
    }

    public AABBStaticNode() { }

    public AABBStaticNode(BoundingBox myBox)
    {
        Bounds = myBox;
    }
}