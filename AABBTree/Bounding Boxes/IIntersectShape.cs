using System.Numerics;

public interface IIntersectShape
{
    bool Intersects(BoundingBox box);
    bool Intersects(BoundingFrustum frustum);
    bool Intersects(BoundingSphere sphere);
    PlaneIntersectionType Intersects(Plane plane);
    float? Intersects(Ray ray);
}