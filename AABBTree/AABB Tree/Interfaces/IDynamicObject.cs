using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDynamicObject
{
    ValueObserver<BoundingBox> Bounds { get; }
}