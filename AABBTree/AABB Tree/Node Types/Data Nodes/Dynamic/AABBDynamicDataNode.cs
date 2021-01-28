using System;
using System.Collections.Generic;

[Serializable]
public partial class SimpleAABBTree
{
    [Serializable]
    public class AABBDynamicDataNode : IAABBNode, IAABBTreeDataHolder
    {

        ValueObserver<BoundingBox> myInternalItem;
        public AABBDynamicDataNode(ValueObserver<BoundingBox> myBox, SimpleAABBTree myTree, IDynamicObject myData)
        {
            Owner = myTree;
            this.Data = myData;
            myInternalItem = myBox;
            myInternalItem.AddObserver(HasChangedValue);
        }

        IDynamicObject InternalData;
        public IDynamicObject Data
        {
            get
            {
                return InternalData;
            }
            set
            {
                myInternalItem?.RemoveObserver(this.HasChangedValue);
                InternalData = value;
                myInternalItem = InternalData.Bounds;
                myInternalItem.AddObserver(this.HasChangedValue);
            }
        }

        public SimpleAABBTree Owner{ get; set; }
        public int DepthOffset{ get; set; }
        public IAABBNode Left{ get; set; }
        public IAABBNode Parent{ get; set; }
        public IAABBNode Right{ get; set; }
        public BoundingBox Bounds { get { return myInternalItem.Value; } set { } }

        private void HasChangedValue(BoundingBox oldValue, BoundingBox newValue)
        {
            if (oldValue == null) return;
            if (oldValue == newValue) return;
            Owner.RemoveNode(this);

            Owner.Insert(this);
        }

        public bool ContainsData()
        {
            return true;
        }

        public void Visit(IAABBTreeVistor T)
        {
            T.ProcessNodeData(this);
        }

        public object GetInternalData()
        {
            return this.Data;
        }
    }


    internal void RemoveIfRoot(AABBStaticNode aABBStaticNode)
    {
        throw new NotImplementedException();
    }
}
