using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
public static class PlaneExtensionMethods
{

    public static PlaneIntersectionType Intersects(this Plane G, BoundingBox box)
    {
        return box.Intersects(G);
    }

    public static void Intersects(this Plane G, ref BoundingBox box, out PlaneIntersectionType result)
    {
        box.Intersects(ref G, out result);
    }

    public static PlaneIntersectionType Intersects(this Plane G, BoundingFrustum frustum)
    {
        return frustum.Intersects(G);
    }

    public static PlaneIntersectionType Intersects(this Plane G, BoundingSphere sphere)
    {
        return sphere.Intersects(G);
    }

    public static void Intersects(this Plane G, ref BoundingSphere sphere, out PlaneIntersectionType result)
    {
        sphere.Intersects(ref G, out result);
    }

    public static PlaneIntersectionType Intersects(this Plane G, ref Vector3 point)
    {

        float distance = Plane.DotCoordinate(G, point);

        if (distance > 0)
            return PlaneIntersectionType.Front;

        if (distance < 0)
            return PlaneIntersectionType.Back;

        return PlaneIntersectionType.Intersecting;
    }


    /// <summary>
    /// Returns a value indicating what side (positive/negative) of a plane a point is
    /// </summary>
    /// <param name="point">The point to check with</param>
    /// <param name="plane">The plane to check against</param>
    /// <returns>Greater than zero if on the positive side, less than zero if on the negative size, 0 otherwise</returns>
    public static float ClassifyPoint(this Plane plane, ref Vector3 point)
    {
        return point.X * plane.Normal.X + point.Y * plane.Normal.Y + point.Z * plane.Normal.Z + plane.D;
    }

    /// <summary>
    /// Returns the perpendicular distance from a point to a plane
    /// </summary>
    /// <param name="point">The point to check</param>
    /// <param name="plane">The place to check</param>
    /// <returns>The perpendicular distance from the point to the plane</returns>
    public static float PerpendicularDistance(this Plane plane, ref Vector3 point)
    {
        // dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
        return (float)Math.Abs((plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z)
                                / Math.Sqrt(plane.Normal.X * plane.Normal.X + plane.Normal.Y * plane.Normal.Y + plane.Normal.Z * plane.Normal.Z));
    }


}