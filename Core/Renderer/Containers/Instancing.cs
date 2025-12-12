using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Renderer.Modules;

namespace Manifold.Core.Renderer.Containers;
public struct InstanceBatchKey
{
    public Mesh Mesh;
    public Material Material;

    public InstanceBatchKey(Mesh mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }

    public override int GetHashCode() => HashCode.Combine(Mesh, Material);
    public override bool Equals(object? obj) =>
        obj is InstanceBatchKey other && other.Mesh == Mesh && other.Material == Material;
}