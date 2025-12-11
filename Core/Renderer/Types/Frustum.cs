using Manifold.Core.Renderer.Maths;
using OpenTK.Mathematics;

namespace Manifold.Core.Renderer.Types;

public struct Frustum {
    public Plane Left, Right, Bottom, Top, Near, Far;

    public static Frustum FromMatrix(Matrix4 vp) {
        Frustum f;

        // Rows
        Vector4 r0 = vp.Row0;
        Vector4 r1 = vp.Row1;
        Vector4 r2 = vp.Row2;
        Vector4 r3 = vp.Row3;

        f.Left = MakePlane(r3 + r0);
        f.Right = MakePlane(r3 - r0);
        f.Bottom = MakePlane(r3 + r1);
        f.Top = MakePlane(r3 - r1);
        f.Near = MakePlane(r3 + r2);
        f.Far = MakePlane(r3 - r2);
        
        return f;
    }
    
    private static Plane MakePlane(Vector4 v) {
        var plane = new Plane(
            new Vector3(v.X, v.Y, v.Z),
            v.W
        );
        plane.Normalize();

        return plane;
    }
    
    public static bool IsAABBOutsidePlane(in AABB aabb, in Plane plane)
    {
        Vector3 p;

        p.X = plane.Normal.X >= 0 ? aabb.Max.X : aabb.Min.X;
        p.Y = plane.Normal.Y >= 0 ? aabb.Max.Y : aabb.Min.Y;
        p.Z = plane.Normal.Z >= 0 ? aabb.Max.Z : aabb.Min.Z;

        return plane.GetSignedDistance(p) < 0;
    }

    public static bool IsVisible(in AABB bounds, in Frustum frustum)
    {
        if (IsAABBOutsidePlane(bounds, frustum.Left)) return false;
        if (IsAABBOutsidePlane(bounds, frustum.Right)) return false;
        if (IsAABBOutsidePlane(bounds, frustum.Top)) return false;
        if (IsAABBOutsidePlane(bounds, frustum.Bottom)) return false;
        if (IsAABBOutsidePlane(bounds, frustum.Near)) return false;
        if (IsAABBOutsidePlane(bounds, frustum.Far)) return false;

        return true;
    }
}