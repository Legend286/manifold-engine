using OpenTK.Mathematics;

namespace Manifold.Core.Renderer.Maths;

public struct AABB {
    public Vector3 Min;
    public Vector3 Max;

    public AABB(Vector3 min, Vector3 max) {
        Min = min;
        Max = max;
    }

    public void Encapsulate(Vector3 p) {
        Min = Vector3.ComponentMin(Min, p);
        Max = Vector3.ComponentMax(Max, p);
    }

    public Vector3 Center => (Min + Max) * 0.5f;

    public Vector3 Extents => (Max - Min) * 0.5f;

    public static AABB TransformAABB(in AABB local, in Matrix4 world) {
        // Transform center and extents separately
        Vector3 center = (local.Min + local.Max) * 0.5f;
        Vector3 extents = (local.Max - local.Min) * 0.5f;

        // Transform the center
        Vector3 worldCenter = Vector3.TransformPosition(center, world);

        // Transform the extents by abs(M3x3) * extents
        Matrix3 absMat = new Matrix3(
            MathF.Abs(world.M11), MathF.Abs(world.M12), MathF.Abs(world.M13),
            MathF.Abs(world.M21), MathF.Abs(world.M22), MathF.Abs(world.M23),
            MathF.Abs(world.M31), MathF.Abs(world.M32), MathF.Abs(world.M33)
        );

        Vector3 worldExtents = absMat * extents;

        return new AABB(
            worldCenter - worldExtents,
            worldCenter + worldExtents
        );
    }
}
