using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class BoundingPerspectiveFustrum : BoundingFrustum
{
    public BoundingPerspectiveFustrum(ref Vector3 position, ref Quaternion orientation, ref Vector3 forward, ref Vector3 up, float fov, float nearClipPlane, float farClipPlane, float aspect)
    {
        this.Up = up;
        this.Forward = forward;
        RebuildPerspective(ref position, ref orientation, ref forward, ref up, fov, nearClipPlane, farClipPlane, aspect);
    }

    private void RebuildPerspective(ref Vector3 position, ref Quaternion orientation, ref Vector3 forward, ref Vector3 up, float fov, float nearClipPlane, float farClipPlane, float aspect)
    {
        Matrix4x4 Perspective = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspect, nearClipPlane, farClipPlane);
        Matrix4x4 View = Matrix4x4.CreateLookAt(position, position + forward, up);
        Matrix4x4 Rotation = Matrix4x4.CreateFromQuaternion(orientation);
        View *= Rotation;
        View *= Perspective;
        InitializeWithValue(View);
    }

    public Vector3 Up { get; }
    public Vector3 Forward { get; }
}