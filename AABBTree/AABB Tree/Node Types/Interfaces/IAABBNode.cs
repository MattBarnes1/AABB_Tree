using System;

public interface IAABBNode : IAcceptAABBTreeVisitor
{
    SimpleAABBTree Owner { get; set; }
    int DepthOffset { get; set; }
    IAABBNode Left { get; set; }
    IAABBNode Parent { get; set; }
    IAABBNode Right { get; set; }
    bool ContainsData();
    BoundingBox Bounds { get; set;}

}