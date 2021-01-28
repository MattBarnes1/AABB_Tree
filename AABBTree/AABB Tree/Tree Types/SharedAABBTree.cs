using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class SharedAABBTree : SimpleAABBTree
{
    
    public SharedAABBTree(SimpleAABBTree ParentObject, IAABBNode minimalSharedBranch) : base(minimalSharedBranch)
    {
        this.ParentObject = ParentObject;
    }

    public SimpleAABBTree ParentObject { get; }
}