using System.Numerics;

public interface IBoundingShape
{
    ContainmentType Contains(IBoundingShape myShape);
    ContainmentType Contains(BoundingBox box);
    ContainmentType Contains(BoundingFrustum frustum);
    ContainmentType Contains(BoundingSphere sphere);
    ContainmentType Contains(Vector3 point);
}