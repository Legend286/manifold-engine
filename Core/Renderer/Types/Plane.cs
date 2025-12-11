using OpenTK.Mathematics;

namespace Manifold.Core.Renderer.Types;

public struct Plane
{
    public Vector3 Normal;
    public float Distance;

    public Plane(Vector3 normal, float distance)
    {
        Normal = normal;
        Distance = distance;
    }

    public float GetSignedDistance(Vector3 point)
    {
        return Vector3.Dot(Normal, point) + Distance;
    }

    public void Normalize()
    {
        float length = Normal.Length;
        Normal /= length;
        Distance /= length;
    }
}
