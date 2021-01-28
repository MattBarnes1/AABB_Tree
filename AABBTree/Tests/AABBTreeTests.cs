using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class SimpleAABBTree_Tests
{
    /* AABB tree Root is null and search requests
     AABB tree Root is One Item and search requests
     Dynamic part of SimpleAABBTree Inserts, Removals, and Reinserts
     SimpleAABBTree Get Nearest*/

    public class SimpleAABBTreeStaticOBJ : IStaticObject
    {
        public BoundingBox Bounds { get; set; }
        public SimpleAABBTreeStaticOBJ(BoundingBox aBounds)
        {
            Bounds = aBounds;
        }
    }
    public class SimpleAABBTreeDynamicInstance : IDynamicObject
    {
        public ValueObserver<BoundingBox> Bounds { get; set; } = new ValueObserver<BoundingBox>();
        public SimpleAABBTreeDynamicInstance(BoundingBox aBounds)
        {
            Bounds.Value = aBounds;
        }
    }
    #region SimpleAABBTree Utility Function Tests
    [Test(Description = "Compare To Checks, creates two trees. TreeB.CompareTo(TreeB) should return 0;")]
    public void SimpleAABBTree_CompareToCheck_SameTree()
    {
        SimpleAABBTree myATree = new SimpleAABBTree();
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            var myEntity = new SimpleAABBTreeDynamicInstance(new BoundingBox(new Vector3(myRandom.Next(-100, -1), myRandom.Next(-100, -1), myRandom.Next(-100, -1)), new Vector3(myRandom.Next(0, 100), myRandom.Next(0, 100), myRandom.Next(0, 100))));
            myATree.Insert(myEntity);
        }
        Assert.AreEqual(myBTree.CompareTo(myBTree), 0);
    }

    [Test(Description = "Compare To Checks, creates two trees. Tree A hass all the same items as tree B except 1. TreeA.CompareTo(TreeB) should return 1, and inverse should return -1;")]
    public void SimpleAABBTree_CompareToCheck_DifferentSizedTrees_SameItems()
    {
        SimpleAABBTree myATree = new SimpleAABBTree();
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        object myObjectToRemove = null;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            var myEntity = new SimpleAABBTreeDynamicInstance(new BoundingBox(new Vector3(myRandom.Next(-100, -1), myRandom.Next(-100, -1), myRandom.Next(-100, -1)), new Vector3(myRandom.Next(0, 100), myRandom.Next(0, 100), myRandom.Next(0, 100))));
            if (i + 1 != TESTCOUNT)
            {
                myATree.Insert(myEntity);
            }
            myBTree.Insert(myEntity);
        }
        Assert.AreEqual(1, myATree.CompareTo(myBTree));
        Assert.AreEqual(-1, myBTree.CompareTo(myATree));
    }

    [Test(Description = "Compare To Checks, creates two trees. Tree A hass all the same items as tree B except 1. Remove that item from the AABB Tree and RebuildTree. CompareTo should return 0;")]
    public void SimpleAABBTree_RemoveRebuildCheck()
    {
        SimpleAABBTree myATree = new SimpleAABBTree();
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        SimpleAABBTreeDynamicInstance myObjectToRemove = null;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            var myEntity = new SimpleAABBTreeDynamicInstance(new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100))));
            if (i + 1 != TESTCOUNT)
            {
                myATree.Insert(myEntity);
            }
            else
            {
                myObjectToRemove = myEntity;
            }
            myBTree.Insert(myEntity);
        }
        myBTree.Remove(myObjectToRemove);
        myBTree.Rebuild();
        Assert.AreEqual(myATree.CompareTo(myBTree), 0);
    }

    [Test(Description = "Checks that if we delete a node the count for the tree goes down.")]
    public void SimpleAABBTree_Deletion_CheckCountIsZero()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2)));

        myTree.Insert(myEntity);
        myTree.Remove(myEntity);
        Assert.IsTrue(myTree.Count == 1);
    }

    [Test(Description = "Checks that if we add a node and delete a node we go back to count 0.")]
    public void SimpleAABBTree_Deletion_CheckCountIsZeroAdd1Remove1()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        Assert.IsTrue(myTree.Count == 1);
        myTree.Remove(myEntity);
        Assert.IsTrue(myTree.Count == 0);
    }

    #endregion

    #region SimpleAABBTree Iterator Tests
    Random myRandom = new Random();
    [Test(Description = "Adds multiple entries (same class) to the SimpleAABBTree and iterates over it with the SimpleAABBTree Iterator.")]
    public void SimpleAABBTree_IteratorTest_MultipleEntities_OneEntityDifferentClass_Added()
    {
        int TESTCOUNT = 20;
        SimpleAABBTree myTree = new SimpleAABBTree();
        BoundingBox myBox;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            var myEntity = new SimpleAABBTreeDynamicInstance(new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100))));
            myTree.Insert(myEntity);
        }
        //Inserts one dummy item
        SimpleAABBTreeStaticOBJ myEntity2 = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100))));
        myTree.Insert(myEntity2);

        int Count = 0;
        foreach (SimpleAABBTreeDynamicInstance A in myTree.GetEnumerator<SimpleAABBTreeDynamicInstance>())
        {
            Count++;
        }
        Assert.IsTrue(Count == TESTCOUNT);
    }

    [Test(Description = "Adds multiple entries (same class) to the SimpleAABBTree and iterates over it with the SimpleAABBTree Iterator.")]
    public void SimpleAABBTree_IteratorTest_MultipleSameEntityAdded()
    {
        int TESTCOUNT = 20;
        SimpleAABBTree myTree = new SimpleAABBTree();
        for (int i = 0; i < TESTCOUNT; i++)
        {
            var myEntity = new SimpleAABBTreeDynamicInstance(new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100))));
            myTree.Insert(myEntity);
        }
        int Count = 0;
        foreach (SimpleAABBTreeDynamicInstance A in myTree.GetEnumerator<SimpleAABBTreeDynamicInstance>())
        {
            Count++;
        }
        Assert.IsTrue(Count == TESTCOUNT);
    }

    [Test(Description = "Adds one entry to the SimpleAABBTree and iterates over it with the SimpleAABBTree Iterator.")]
    public void SimpleAABBTree_IteratorTest_OneEntityAdded()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeDynamicInstance(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        bool Worked = false;
        foreach (SimpleAABBTreeDynamicInstance A in myTree.GetEnumerator<SimpleAABBTreeDynamicInstance>())
        {
            Worked = true;
            break;
        }
        Assert.IsTrue(Worked);
    }

    [Test(Description = "Adds one entry to the SimpleAABBTree and sees if count updated.")]
    public void SimpleAABBTree_CountTest_OneEntityAdded()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        Assert.AreEqual(myTree.Count, 1);
    }
    #endregion

    #region SimpleAABBTree Insert/Remove

    [Test(Description = "Checks that SimpleAABBTree returns one box from a set of two disjointed boxes.")]
    public void SimpleAABBTree_InsertTwoElements_NoOverlap_TreeCheck()
    {//TODO: STILL
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(1.1f, 1.1f, 1.1f), new Vector3(2, 2, 2)));
        BoundingBox myBox = new BoundingBox(new Vector3(1.1f, 1.1f, 1.1f), new Vector3(2, 2, 2));
        myTree.Insert(myEntity);
        var Results = myTree.BoundingPrimativeCast<Object>(myBox);
        Assert.IsTrue(Results.Count == 1);
        bool correctObject = false;
        foreach (object A in Results)
        {
            if (A == myEntity)
            {
                correctObject = true;
            }
        }
        Assert.IsTrue(correctObject);
    }

    [Test(Description = "Checks that SimpleAABBTree returns two intersecting boxes if we sent two in.")]
    public void SimpleAABBTree_InsertTwoElements_Intersects_TreeCheck()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2)));
        BoundingBox myBox = new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2));
        myTree.Insert(myEntity);

        var Results = myTree.BoundingPrimativeCast<Object>(myBox);
        Assert.IsTrue(Results.Count == 2);
        bool correctObject = false;
        foreach (object A in Results)
        {
            if (A == myEntity)
            {
                correctObject = true;
            }
        }
        Assert.IsTrue(correctObject);
    }

    [Test(Description = "Checks that SimpleAABBTree, if given a set of two disjointed boxes, and one is removed, the other can still be found.")]
    public void SimpleAABBTree_InsertTwoDisjointedElementsRemoveOne_ChecksOtherRemains()
    {//TODO: STILL
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        var myEntity2 = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(1.1f, 1.1f, 1.1f), new Vector3(2, 2, 2)));
        BoundingBox myBox = new BoundingBox(new Vector3(1.1f, 1.1f, 1.1f), new Vector3(2, 2, 2));
        myTree.Insert(myEntity2);

        myTree.Remove(myEntity);

        var Results = myTree.BoundingPrimativeCast<Object>(myBox);
        Assert.IsTrue(Results.Count == 1);
        bool correctObject = false;
        foreach (object A in Results)
        {
            if (A == myEntity2)
            {
                correctObject = true;
            }
        }
        Assert.IsTrue(correctObject);
    }

    [Test(Description = "Checks that SimpleAABBTree, if given a set of two overlapping boxes, and one is removed, the other can still be found.")]
    public void SimpleAABBTree_InsertTwoOverlappingElementsRemoveOne_ChecksOtherRemains()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        BoundingBox myBox = new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2));
        var myEntity2 = new SimpleAABBTreeStaticOBJ(myBox);
        myTree.Insert(myEntity2);

        myTree.Remove(myEntity);

        var Results = myTree.BoundingPrimativeCast<Object>(myBox);
        Assert.AreEqual(1, Results.Count, "Results didn't match expected number of items in the Tree.");
        bool correctObject = false;
        foreach (object A in Results)
        {
            if (A == myEntity2)
            {
                correctObject = true;
            }
        }
        Assert.IsTrue(correctObject);
    }

    #endregion

    #region Raycast Tests
    [Test(Description = "RaycastReturnFirst Detection - Raycasts an empty tree. Returns empty and doesn't throw exception.")]
    public void SimpleAABBTree_TreeRaySingleHitDetection_RunOnEmptyTree()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        Ray aRay = new Ray(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        RayCastHit<Object> myHit = myTree.RaycastReturnFirst<Object>(aRay);
        Assert.AreEqual(myHit.Count, 0);
    }
    [Test(Description = "RaycastReturnFirst Detection - Raycasts and expects to hit one box in a tree with only one box at root.")]
    public void SimpleAABBTree_TreeRaySingleHitDetection_OneItemInTree()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        BoundingBox myBox = new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        var myEntity = new SimpleAABBTreeStaticOBJ(myBox);
        myTree.Insert(myEntity);
        Ray aRay = new Ray(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        RayCastHit<Object> myHit = myTree.RaycastReturnFirst<Object>(aRay);
        Assert.AreEqual(myHit.Count, 1);
    }

    [Test(Description = "Raycast Detection - Checks raycast count after casting.")]
    public void SimpleAABBTree_TreeRayHitDetection_CheckCountIsAmount()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2)));
        myTree.Insert(myEntity);

        Ray aRay = new Ray(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        RayCastHit<Object> myHit = myTree.Raycast<Object>(aRay);
        Assert.AreEqual(myHit.Count, 1);
        aRay = new Ray(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 1, 0));
        myHit = myTree.Raycast<Object>(aRay);
        Assert.AreEqual(myHit.Count, 2);
        aRay = new Ray(new Vector3(0.5f, -1, 0.5f), new Vector3(0, 1, 0));
        myHit = myTree.Raycast<Object>(aRay);
        Assert.AreEqual(myHit.Count, 2);
        aRay = new Ray(new Vector3(1.5f, 1.5f, 1.5f), new Vector3(0, 1, 0));
        myHit = myTree.Raycast<Object>(aRay);
        Assert.AreEqual(myHit.Count, 1);

    }
    [Test(Description = "RaycastReturnFirst Detection - Raycasts and expects to hit one box.")]
    public void SimpleAABBTree_TreeRaySingleHitDetection_CheckIsOne()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2)));
        myTree.Insert(myEntity);

        Ray aRay = new Ray(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        RayCastHit<Object> myHit = myTree.RaycastReturnFirst<Object>(aRay);
        Assert.AreEqual(myHit.Count, 1);
        aRay = new Ray(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 1, 0));
        myHit = myTree.RaycastReturnFirst<Object>(aRay);
        Assert.AreEqual(myHit.Count, 1);
        aRay = new Ray(new Vector3(0.5f, -1, 0.5f), new Vector3(0, 1, 0));
        myHit = myTree.RaycastReturnFirst<Object>(aRay);
        Assert.AreEqual(myHit.Count, 1);
        aRay = new Ray(new Vector3(1.5f, 1.5f, 1.5f), new Vector3(0, 1, 0));
        myHit = myTree.RaycastReturnFirst<Object>(aRay);
        Assert.AreEqual(myHit.Count, 1);
    }
    #endregion

    #region Primative Casting

    [Test(Description = "Primative Cast Detection - Raycasts with bounding box primative on an empty tree. Returns empty and doesn't throw an exception.")]
    public void SimpleAABBTree_BoundingBoxHitDetection_HitOne_TreeOnlyHasNone()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();

        BoundingBox myTestBox = new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(2, 2, 2));
        RayCastHit<Object> myHit = myTree.BoundingPrimativeCast<Object>(myTestBox);
        Assert.AreEqual(0, myHit.Count);
    }

    [Test(Description = "Primative Cast Detection - Raycasts with bounding box primative and expects to hit one in a tree with only one element.")]
    public void SimpleAABBTree_BoundingBoxHitDetection_HitOne_TreeOnlyHasOne()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        BoundingBox myTestBox = new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(2, 2, 2));
        RayCastHit<Object> myHit = myTree.BoundingPrimativeCast<Object>(myTestBox);
        Assert.AreEqual(1, myHit.Count);
    }
    [Test(Description = "Primative Cast Detection - Raycasts with bounding box primative and expects to hit all.")]
    public void SimpleAABBTree_BoundingBoxHitDetection_HitAll()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2)));
        myTree.Insert(myEntity);

        BoundingBox myTestBox = new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(2, 2, 2));

        RayCastHit<Object> myHit = myTree.BoundingPrimativeCast<Object>(myTestBox);
        Assert.AreEqual(myHit.Count, 2);

    }
    [Test(Description = "Primative Cast Detection - Raycasts with bounding box primative and expects to hit one.")]
    public void SimpleAABBTree_BoundingBoxHitDetection_HitOne()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0.5f)));
        myTree.Insert(myEntity);

        BoundingBox myTestBox = new BoundingBox(new Vector3(0.75f, 0.75f, 0.75f), new Vector3(2, 2, 2));
        RayCastHit<Object> myHit = myTree.BoundingPrimativeCast<Object>(myTestBox);
        Assert.AreEqual(1, myHit.Count);
    }

    [Test(Description = "Primative Cast Detection - Raycasts with bounding box primative and expects to hit none.")]
    public void SimpleAABBTree_BoundingBoxHitDetection_HitNone()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        var myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
        myTree.Insert(myEntity);
        myEntity = new SimpleAABBTreeStaticOBJ(new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(2, 2, 2)));
        myTree.Insert(myEntity);
        BoundingBox myTestBox = new BoundingBox(new Vector3(3f, 3f, 3f), new Vector3(4, 4, 4));
        RayCastHit<Object> myHit = myTree.BoundingPrimativeCast<Object>(myTestBox);
        Assert.AreEqual(0, myHit.Count);
    }
    #endregion

    #region GetNearestTests

    public class NearestObjectForSort : IComparable<NearestObjectForSort>
    {
        public float Distance;
        public Object myObject;

        public int CompareTo(NearestObjectForSort other)
        {
            if (other == null) return -1;
            return -other.Distance.CompareTo(Distance); //High to low
        }
    }

    [Test(Description = "GetNearestToPoint - Creates 100 random boxes, sorts the boxes based on distance to search point and uses it to check the GetNearestToPoint Function.")]
    public void SimpleAABBTree_BoundingBoxHitDetection_GetNearestToPoint()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        Vector3 TestPoint = new Vector3(0, 0, 0);
        List<NearestObjectForSort> myObjectsAndDistanceFromPoint = new List<NearestObjectForSort>();
        for (int i = 0; i < 100; i++)
        {
            var aBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeStaticOBJ(aBox);
            NearestObjectForSort mySortObject = new NearestObjectForSort() { Distance = Vector3.Distance(aBox.Center, TestPoint), myObject = myEntity };
            myObjectsAndDistanceFromPoint.Add(mySortObject);
            myTree.Insert(myEntity);
        }

        myObjectsAndDistanceFromPoint.Sort();
        var Result = myTree.GetNearest<SimpleAABBTreeStaticOBJ>(TestPoint, 100);
        Assert.AreEqual(100, Result.Count);
        for (int i = 0; i < Result.Count; i++)
        {
            Assert.IsNotNull(Result[i]);
            //Assert.AreEqual(Vector3.Distance(Result[i].Bounds.Center, TestPoint), myObjectsAndDistanceFromPoint[i].Distance);
        }
        Result = myTree.GetNearest<SimpleAABBTreeStaticOBJ>(TestPoint, 100, 100);
        Assert.AreEqual(100, Result.Count);
        for (int i = 0; i < Result.Count; i++)
        {
            Assert.IsNotNull(Result[i]);
            //Assert.AreEqual(Result[i], myObjectsAndDistanceFromPoint[i]);
        }
    }

    [Test(Description = "GetNearestToPoint - Performs function on empty tree.")]
    public void SimpleAABBTree_BoundingBoxHitDetection_GetNearestToPoint_EmptyTree()
    {
        SimpleAABBTree myTree = new SimpleAABBTree();
        Vector3 TestPoint = new Vector3(0, 0, 0);
        var Result = myTree.GetNearest<Object>(TestPoint, 100);
        Assert.AreEqual(Result.Count, 0);
    }
    #endregion

    #region SimpleAABBTree Exceptions For Readonly Iterators
    [Test(Description = "If we have an iterator active and Rebuild called, throw new exception.")]
    public void Test1()
    {
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeDynamicInstance(myBox);
            myBTree.Insert(myEntity);
        }
        myBTree.SetReadOnly(true);
        Assert.Throws<Exception>(delegate ()
        {
            myBTree.Rebuild();
        });
    }


    [Test(Description = "If we have an iterator active and Insert(IStaticObject) called, throw new exception.")]
    public void Test2()
    {
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeStaticOBJ(myBox);
            myBTree.Insert(myEntity);
        }
        myBTree.SetReadOnly(true);
        Assert.Throws<Exception>(delegate ()
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeStaticOBJ(myBox); 
            myBTree.Insert(myEntity);
        });

    }


    [Test(Description = "If we have an iterator active  and Insert(IDynamicObject), throw new exception.")]
    public void Test3()
    {
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeDynamicInstance(myBox);
            myBTree.Insert(myEntity);
        }
        myBTree.SetReadOnly(true);
        Assert.Throws<Exception>(delegate ()
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeDynamicInstance(myBox);
            myBTree.Insert(myEntity);
        });

    }


    [Test(Description = "If we have an iterator active and Remove(IStaticObject), throw new exception.")]
    public void Test4()
    {
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        SimpleAABBTreeStaticOBJ Last = null;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeStaticOBJ(myBox); //();
            myBTree.Insert(myEntity);
            if( i+1 == TESTCOUNT)
            {
                Last = myEntity;
            }
        }
        myBTree.SetReadOnly(true);
        Assert.Throws<Exception>(delegate ()
        {
            myBTree.Remove(Last);
        });

    }


    [Test(Description = "If we have an iterator active and Remove(IDynamicObject), throw new exception.")]
    public void Test5()
    {
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        SimpleAABBTreeDynamicInstance Last = null;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeDynamicInstance(myBox);
            myBTree.Insert(myEntity);
            if (i + 1 == TESTCOUNT)
            {
                Last = myEntity;
            }
        }
        myBTree.SetReadOnly(true);

        Assert.Throws<Exception>(delegate ()
        {
            myBTree.Remove(Last);
        });

    }


    [Test(Description = "If we have an iterator active and Clear All called, throw new exception.")]
    public void Test6()
    {
        SimpleAABBTree myBTree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeDynamicInstance(myBox);
            myBTree.Insert(myEntity);
        }
        myBTree.SetReadOnly(true);

        Assert.Throws<Exception>(delegate ()
        {
            myBTree.Clear();
        });

    }



    #endregion

    #region Reinitialize From Root Testing
    [Test(Description = "Reinitialize is called for the creation of the static object tree. If they match here, then the ReinitializeTreeFromRoot works.")]
    public void Test7()
    {
        SimpleAABBTree myATree = new SimpleAABBTree();
        BoundingBox myBox;
        int TESTCOUNT = 20;
        SimpleAABBTreeDynamicInstance myObjectToRemove = null;
        for (int i = 0; i < TESTCOUNT; i++)
        {
            myBox = new BoundingBox(new Vector3(myRandom.Next(-100, 0), myRandom.Next(-100, 0), myRandom.Next(-100, 0)), new Vector3(myRandom.Next(1, 100), myRandom.Next(1, 100), myRandom.Next(1, 100)));
            var myEntity = new SimpleAABBTreeDynamicInstance(myBox);
            myEntity.Bounds.Value = myBox;
            myATree.Insert(myEntity);
        }

        StaticAABBTree myStaticObject = myATree.GetAreaAsStaticTree(new BoundingBox(new Vector3(-1000000), new Vector3(100000)));

        Assert.IsTrue(myStaticObject.CompareTo(myATree) == 0);
    }

    #endregion
}

