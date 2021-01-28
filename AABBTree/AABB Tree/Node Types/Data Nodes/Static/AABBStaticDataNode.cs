using System;

[Serializable]
public class AABBStaticDataNode : IAABBNode, IAABBTreeDataHolder
{
    Action<IAABBNode> OnRemovedWatcher { get; }
    public BoundingBox GetBounds()
    {
        return Bounds;
    }

    public void Remove()
    {
        var Parent = this.Parent;
        if (this.Parent.Left == this)
        {
            IAABBNode LastNodeRemoved;
            if (this.Parent.Right == null)
            {
                this.Parent.Left = null;
                int InvalidatedParentCount = RemoveInvalidParents(this.Parent, out LastNodeRemoved);
            }
            else
            {
                this.Parent.DepthOffset -= 1;
            }
        }
        else
        {
            this.Parent.Right = null;
            if (this.Parent.Left == null) //case 1 node;
            {
                this.Parent.Right = null;
                IAABBNode LastNodeRemoved;
                int InvalidatedParentCount = RemoveInvalidParents(this.Parent, out LastNodeRemoved);

            }
            else
            {
                this.Parent.DepthOffset -= 1;
            }
        }
    }

    private int RemoveInvalidParents(IAABBNode NodeBeingRemoved, out IAABBNode lastModifiedNode)
    {
        int Count = 0;
        var myParent = NodeBeingRemoved.Parent;

        if (NodeBeingRemoved.Parent.Left == NodeBeingRemoved)
        {
            NodeBeingRemoved.Parent.Left = null;
        }
        else if (NodeBeingRemoved.Parent.Right == NodeBeingRemoved)
        {
            NodeBeingRemoved.Parent.Right = null;
        }
        lastModifiedNode = NodeBeingRemoved.Parent;


        if (myParent.Right == null && myParent.Left == null) //case 1 node;
        {
            var formerChildNode = myParent;
            myParent = myParent.Parent;
            while (myParent != null)
            {
                if (myParent.Left == formerChildNode)
                {
                    myParent.Left = null;
                    if (myParent.Right == null)
                    {
                        formerChildNode = myParent;
                        myParent = myParent.Parent;
                        Count++;
                    }
                    else
                    {
                        lastModifiedNode = myParent;
                        myParent = null;
                    }
                }
                else
                {
                    myParent.Right = null;
                    if (myParent.Left == null)
                    {
                        Count++;
                        formerChildNode = myParent;
                        myParent = myParent.Parent;
                    }
                    else
                    {
                        lastModifiedNode = myParent;
                        myParent = null;
                    }
                }
            }
        }
        else
        {
            lastModifiedNode = myParent;
        }
        return Count;
    }
    public bool ContainsData() { return true; }

    public void Visit(IAABBTreeVistor T)
    {
        T.ProcessNodeData(this);
    }

    public object GetInternalData()
    {
       return InternalData;
    }

    protected IStaticObject InternalData;

    public AABBStaticDataNode(BoundingBox myBox, SimpleAABBTree myTree, IStaticObject myData)
    {
        Owner = myTree;
        Bounds = myBox;
        this.Data = myData;
    }

    public virtual IStaticObject Data
    {
        get
        {
            return InternalData;
        }
        set
        {
            InternalData = value;
        }
    }

    public SimpleAABBTree Owner{ get; set; }
    public int DepthOffset{ get; set; }
    public IAABBNode Left{ get; set; }
    public IAABBNode Parent{ get; set; }
    public IAABBNode Right{ get; set; }
    public BoundingBox Bounds{ get; set; }
}