using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class BoundingBoxNewConstructorTest
{
    [Test(Description = "Compares the constructor of the rectangular bounding box to one made with a normal constructor" )]
    public void BBNCTTest1()
    {
        BoundingBox myBox = new BoundingBox(-1, 1, new System.Numerics.Vector3(0,0,0));
        List<Vector3> myVectors = new List<Vector3>(myBox.GetCorners());
        Assert.IsTrue(myVectors.Contains(new Vector3(-1, -1, 1)));
        Assert.IsTrue(myVectors.Contains(new Vector3(-1, -1, -1)));
        Assert.IsTrue(myVectors.Contains(new Vector3(-1, 1, 1)));
        Assert.IsTrue(myVectors.Contains(new Vector3(-1, 1, -1)));
        Assert.IsTrue(myVectors.Contains(new Vector3(1, -1, 1)));
        Assert.IsTrue(myVectors.Contains(new Vector3(1, -1, -1)));
        Assert.IsTrue(myVectors.Contains(new Vector3(1, 1, 1)));
        Assert.IsTrue(myVectors.Contains(new Vector3(1, 1, -1)));
    }

    [Test(Description = "Constructs a bounding box at a position and uses the SetCenter function to move the box.")]
    public void BBNCTTest2()
    {
        BoundingBox myBox = new BoundingBox(-1, 1, new System.Numerics.Vector3(0, 0, 0));
        Assert.AreEqual(myBox.Center, ((myBox.Max - myBox.Min) * 0.5f) + myBox.Min);
        BoundingBox myBoxB = new BoundingBox(0, 2, new System.Numerics.Vector3(1, 1, 1));
        Assert.AreEqual(((myBoxB.Max - myBoxB.Min) * 0.5f) + myBoxB.Min, myBoxB.Center);
        myBoxB.SetCenter(new Vector3(0, 0, 0));
        Assert.AreEqual(myBoxB.Center, ((myBoxB.Max - myBoxB.Min) * 0.5f) + myBoxB.Min);
        List<Vector3> myVectorNormal = new List<Vector3>(myBox.GetCorners());
        List<Vector3> myVectorOffset = new List<Vector3>(myBoxB.GetCorners());
        foreach (Vector3 A in myVectorOffset)
        {
            Assert.IsTrue(myVectorNormal.Contains(A));
        }
    }



}