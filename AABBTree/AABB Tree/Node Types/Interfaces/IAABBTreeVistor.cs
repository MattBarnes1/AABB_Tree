using static SimpleAABBTree;

public interface IAABBTreeVistor
{
    void ProcessNodeData(AABBStaticDataNode internalData);
    void ProcessNodeData(AABBDynamicDataNode internalData);
}