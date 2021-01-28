using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public partial class SimpleAABBTree : AABBTreeBase
{
    public SimpleAABBTree(IAABBNode minimalSharedBranch) : base(minimalSharedBranch)
    {
        ReinitializeTreeFromRootItem();

    }

    public void Clear()
    {
        if (IsReadonly) throw new Exception("Someone was either stuck processing or didn't properly dispose of Enumerator for AABBTree.");
        DynamicNodeLookup.Clear();
        StaticNodeLookup.Clear();
        Root = null;
    }



    public SimpleAABBTree()
    {

    }



    /*Branch – Our branches always have exactly two children (known as left and right) and are assigned an AABB that is large enough to contain all of it’s descendants.
Leaf – Our leaves are associated with a game world object and through that have an AABB. A leaf’s AABB must fit entirely within it’s parents AABB and due to how our branches are defined that means it fits in every ancestors AABB.
Root – Our root may be a branch or a leaf*/


    public virtual void Insert(IStaticObject myStaticObject)
    {
        if (IsReadonly) throw new Exception("Someone was either stuck processing or didn't properly dispose of Enumerator for AABBTree.");
        IAABBNode myNewNode = new AABBStaticDataNode(myStaticObject.Bounds, this, myStaticObject);
        this.StaticNodeLookup.Add(myStaticObject, myNewNode);
        Insert(myNewNode);
    }

    public virtual void Insert(IDynamicObject input)
    {
        if (IsReadonly) throw new Exception("Someone was either stuck processing or didn't properly dispose of Enumerator for AABBTree.");
        ValueObserver<BoundingBox> Bounds = input.Bounds;
        IAABBNode myNewNode = new AABBDynamicDataNode(Bounds, this, input);
        DynamicNodeLookup.Add(input, myNewNode);
        Insert(myNewNode);
    }

    private void Insert(IAABBNode myNewNode)
    {
        if (Root == null)
        {
            Root = myNewNode;
            return;
        }
        IAABBNode CurrentObject = Root;
        while (CurrentObject != null)
        {
            if (CurrentObject.ContainsData()) //It's a game object
            {
                MergeIntoBranch(myNewNode, CurrentObject);
                return;
            }
            else //It's just a branch.
            {
                ResizeBranch(myNewNode, CurrentObject);
                ContainmentType? leftContainment = null;
                ContainmentType? rightContainment = null;
                if (CurrentObject.Left != null)
                    leftContainment = CurrentObject.Left.Bounds.Contains(myNewNode.Bounds);
                if (CurrentObject.Right != null)
                    rightContainment = CurrentObject.Right.Bounds.Contains(myNewNode.Bounds);

                if (leftContainment == ContainmentType.Contains || !rightContainment.HasValue || (leftContainment == ContainmentType.Intersects && rightContainment == ContainmentType.Disjoint))
                {
                    CurrentObject = CurrentObject.Left;
                }
                else if (rightContainment == ContainmentType.Contains || !leftContainment.HasValue || (rightContainment == ContainmentType.Intersects && leftContainment == ContainmentType.Disjoint))
                {
                    CurrentObject = CurrentObject.Right;
                }
                else if(CurrentObject.Left != null && CurrentObject.Right != null)
                {
                    CurrentObject = GetNextByHeuristic(myNewNode, CurrentObject);
                }
            }
        }
    }

    public void Rebuild()
    {
        if (IsReadonly) throw new Exception("Someone was either stuck processing or didn't properly dispose of Enumerator for AABBTree.");
        DynamicNodeLookup.Clear();
        StaticNodeLookup.Clear();
        base.ReinitializeTreeFromRootItem();
    }

    private static void ResizeBranch(IAABBNode myNewNode, IAABBNode CurrentObject)
    {
        var ContainmentTypeReturned = CurrentObject.Bounds.Contains(myNewNode.Bounds);
        if (ContainmentTypeReturned == ContainmentType.Intersects || ContainmentTypeReturned == ContainmentType.Disjoint)
        {
            CurrentObject.Bounds = BoundingBox.CreateMerged(myNewNode.Bounds, CurrentObject.Bounds);
        }
    }

    private IAABBNode GetNextByHeuristic(IAABBNode myNewNode, IAABBNode CurrentObject)
    {
        var areabounds = myNewNode.Bounds.Center;
        return GetNextByHeuristice(CurrentObject, areabounds); 
    }

    private static IAABBNode GetNextByHeuristice(IAABBNode CurrentObject, Vector3 areabounds)
    {
        float DistanceLeft = -1;//TODO: Refactor this
        float DistanceRight = -1;
        DistanceLeft = DistanceApproximation.DistanceTaxiCab(CurrentObject.Left.Bounds.Center, areabounds);
        DistanceRight = DistanceApproximation.DistanceTaxiCab(CurrentObject.Right.Bounds.Center, areabounds);
        if (DistanceLeft >= DistanceRight)
        {
            CurrentObject = CurrentObject.Right;
        }
        else
        {
            CurrentObject = CurrentObject.Left;
        }

        return CurrentObject;
    }


    private void MergeIntoBranch(IAABBNode myNewNode, IAABBNode currentObject)
    {
        IAABBNode myNewBranch = new AABBStaticNode();

        myNewBranch.Bounds = BoundingBox.CreateMerged(myNewNode.Bounds, currentObject.Bounds);
        myNewBranch.Left = currentObject;
        myNewBranch.DepthOffset = currentObject.DepthOffset;
        currentObject.DepthOffset--;
        myNewBranch.Right = myNewNode; //Depth Offset = 0;
        myNewNode.Parent = myNewBranch;
        if (currentObject.Parent == null)
        {
            Root = myNewBranch;
        }
        else if (currentObject.Parent.Right == currentObject)
        {
            currentObject.Parent.Right = myNewBranch;
        }
        else if (currentObject.Parent.Left == currentObject)
        {
            currentObject.Parent.Left = myNewBranch;
        }
        myNewBranch.Parent = currentObject.Parent;
        currentObject.Parent = myNewBranch;
    }

    public bool Remove(IDynamicObject MyItem)
    {
        if (IsReadonly) throw new Exception("Someone was either stuck processing or didn't properly dispose of Enumerator for AABBTree.");
        IAABBNode NodeData;
        if (DynamicNodeLookup.TryGetValue((Object)MyItem, out NodeData))
        {
            base.RemoveNode(NodeData);
            DynamicNodeLookup.Remove((Object)MyItem);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Remove(IStaticObject MyItem)
    {
        if (IsReadonly) throw new Exception("Someone was either stuck processing or didn't properly dispose of Enumerator for AABBTree.");
        IAABBNode NodeData;
        if (StaticNodeLookup.TryGetValue((Object)MyItem, out NodeData))
        {
            base.RemoveNode(NodeData);
            StaticNodeLookup.Remove((Object)MyItem);
            return true;
        }
        else
        {
            return false;
        }
    }

    







    public SharedAABBTree GetAreaAsChildTree(BoundingBox Areabounds)
    {
        IAABBNode CurrentObject = Root;
        IAABBNode MinimalSharedBranch = GetMinimalBranch(Areabounds);
        return new SharedAABBTree(this, MinimalSharedBranch);
    }


}
