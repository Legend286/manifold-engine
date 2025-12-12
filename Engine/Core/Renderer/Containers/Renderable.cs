using Manifold.Core.Renderer.Modules;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Renderer.Maths;
using Manifold.Core.SceneSystem;
using OpenTK.Mathematics;

namespace Manifold.Core.Renderer.Containers;

public class Renderable
{
    public Mesh Mesh;
    public Material Material;
    public Transform Transform;
    public AABB RenderBounds;

    public Renderable(
        Mesh mesh,
        Material material,
        Transform transform)
    {
        Mesh = mesh;
        Material = material;
        Transform = transform;
    }

    public AABB GetRenderBounds() {
        // Local-space AABB from mesh
        var local = Mesh.Bounds;

        // Get transform matrix (TRS)
        Matrix4 world = Transform.GetLocalMatrix();

        // Compute local-space corners
        Span<Vector3> corners = stackalloc Vector3[8];
        corners[0] = new Vector3(local.Min.X, local.Min.Y, local.Min.Z);
        corners[1] = new Vector3(local.Max.X, local.Min.Y, local.Min.Z);
        corners[2] = new Vector3(local.Min.X, local.Max.Y, local.Min.Z);
        corners[3] = new Vector3(local.Max.X, local.Max.Y, local.Min.Z);
        corners[4] = new Vector3(local.Min.X, local.Min.Y, local.Max.Z);
        corners[5] = new Vector3(local.Max.X, local.Min.Y, local.Max.Z);
        corners[6] = new Vector3(local.Min.X, local.Max.Y, local.Max.Z);
        corners[7] = new Vector3(local.Max.X, local.Max.Y, local.Max.Z);

        // Transform and refit AABB
        Vector3 min = new Vector3(float.PositiveInfinity);
        Vector3 max = new Vector3(float.NegativeInfinity);

        for (int i = 0; i < 8; i++)
        {
            Vector3 worldPos = Vector3.TransformPosition(corners[i], world);
            min = Vector3.ComponentMin(min, worldPos);
            max = Vector3.ComponentMax(max, worldPos);
        }

        return new AABB(min, max);
    }
}