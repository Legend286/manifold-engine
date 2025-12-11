using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.Maths;
using OpenTK.Mathematics;

namespace Manifold.Core.Renderer.Debug;

public struct DebugVertex {
    public Vector3 Position;
    public Vector4 Color;

    public static readonly BufferLayout Layout = new BufferLayout(
        new BufferElement(ShaderDataType.Float3, "a_Position"), 
        new BufferElement(ShaderDataType.Float4, "a_Color"));
}

public static class DebugDraw {
    internal static readonly List<DebugVertex> Vertices = new();

    public static void Clear() => Vertices.Clear();

    public static void Line(Vector3 a, Vector3 b, Vector4 color) {
        Vertices.Add(new DebugVertex { Position = a, Color = new Vector4(color.Xyz, 0.5f) });
        Vertices.Add(new DebugVertex { Position = b, Color = new Vector4(color.Xyz, 0.5f) });
    }

    public static void AABB(AABB bounds, Vector4 color) {
        AABB(bounds.Min, bounds.Max, color);
    }

    public static void AABB(Vector3 min, Vector3 max, Vector4 color) {
        Vector3[] c =
        {
            new(min.X, min.Y, min.Z),
            new(max.X, min.Y, min.Z),
            new(max.X, max.Y, min.Z),
            new(min.X, max.Y, min.Z),
            new(min.X, min.Y, max.Z),
            new(max.X, min.Y, max.Z),
            new(max.X, max.Y, max.Z),
            new(min.X, max.Y, max.Z),
        };

        int[,] e =
        {
            {0,1},{1,2},{2,3},{3,0},
            {4,5},{5,6},{6,7},{7,4},
            {0,4},{1,5},{2,6},{3,7}
        };

        for (int i = 0; i < e.GetLength(0); i++)
            Line(c[e[i,0]], c[e[i,1]], color);
    }
    
    public static void Transform(
        Vector3 position,
        Quaternion rotation,
        float size = 1.0f) {

        Line(position,
            position + Vector3.Transform(Vector3.UnitX * size, rotation),
            new Vector4(1, 0, 0, 1));

        Line(position,
            position + Vector3.Transform(Vector3.UnitY * size, rotation),
            new Vector4(0, 1, 0, 1));

        Line(position,
            position + Vector3.Transform(Vector3.UnitZ * size, rotation),
            new Vector4(0, 0, 1, 1));
    }

    public static void Point(Vector3 p, float size, Vector4 color) {
        Line(p - Vector3.UnitX * size, p + Vector3.UnitX * size, color);
        Line(p - Vector3.UnitY * size, p + Vector3.UnitY * size, color);
        Line(p - Vector3.UnitZ * size, p + Vector3.UnitZ * size, color);
    }
    
    static void DrawCircle(
        Vector3 center,
        Vector3 axisA,
        Vector3 axisB,
        float radius,
        Vector4 color,
        int segments) {

        float step = MathF.Tau / segments;
        Vector3 prev = center + axisA * radius;

        for (int i = 1; i <= segments; i++) {
            float a = step * i;
            Vector3 next =
                center + (axisA * MathF.Cos(a) + axisB * MathF.Sin(a)) * radius;
            Line(prev, next, color);
            prev = next;
        }
    }
    
    private static readonly Vector3[] _prevRing = new Vector3[256];

    private static void DrawHemisphere(
        Vector3 center,
        Vector3 up,
        float radius,
        Vector4 color,
        int segments)
    {
        segments = Math.Max(8, segments);
        int rings = segments / 2;

        // --- Build stable orthonormal basis ---
        Vector3 right = Vector3.Cross(up, Vector3.UnitZ);
        if (right.LengthSquared < 0.0001f)
            right = Vector3.Cross(up, Vector3.UnitX);
        right.Normalize();

        Vector3 forward = Vector3.Cross(right, up);

        float thetaStep = MathF.Tau / segments;
        float phiStep = (MathF.PI * 0.5f) / rings;

        bool hasPrevRing = false;

        for (int r = 0; r <= rings; r++)
        {
            float phi = r * phiStep;

            Span<Vector3> ring = stackalloc Vector3[segments];

            float sinPhi = MathF.Sin(phi);
            float cosPhi = MathF.Cos(phi);

            for (int i = 0; i < segments; i++)
            {
                float theta = i * thetaStep;

                float cosTheta = MathF.Cos(theta);
                float sinTheta = MathF.Sin(theta);

                Vector3 dir =
                    right   * (cosTheta * cosPhi) +
                    forward * (sinTheta * cosPhi) +
                    up      * sinPhi;

                ring[i] = center + dir * radius;
            }

            // --- Draw ring loop ---
            for (int i = 0; i < segments; i++)
            {
                Line(ring[i], ring[(i + 1) % segments], color);
            }

            // --- Connect to previous ring ---
            if (hasPrevRing)
            {
                for (int i = 0; i < segments; i++)
                {
                    Line(_prevRing[i], ring[i], color);
                }
            }

            // Save ring
            for (int i = 0; i < segments; i++)
                _prevRing[i] = ring[i];

            hasPrevRing = true;
        }
    }

    public static void HemiCircle(
        Vector3 center,
        Vector3 normal,
        Vector3 direction,
        float radius,
        Vector4 color,
        int segments = 16)
    {
        normal = normal.Normalized();
        direction = Vector3.Normalize(direction - Vector3.Dot(direction, normal) * normal);

        Vector3 bitangent = Vector3.Cross(normal, direction);

        float angleStep = MathF.PI / segments;
        Vector3 prev = center + direction * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 next =
                center +
                (direction * MathF.Cos(angle) +
                 bitangent * MathF.Sin(angle)) * radius;

            Line(prev, next, color);
            prev = next;
        }
    }


    public static void Capsule(
        Vector3 start,
        Vector3 end,
        float radius,
        Vector4 color,
        int segments = 12)
    {
        Vector3 axis = end - start;
        float length = axis.Length;

        if (length < 0.0001f)
            return;

        Vector3 dir = axis / length;

        // Build stable perpendicular basis
        Vector3 tangent = Vector3.Cross(dir, Vector3.UnitZ);
        if (tangent.LengthSquared < 0.0001f)
            tangent = Vector3.Cross(dir, Vector3.UnitX);
        tangent.Normalize();

        Vector3 bitangent = Vector3.Cross(dir, tangent);

        // Draw capsule side lines (4 edges for better clarity)
        Line(start + tangent * radius, end + tangent * radius, color);
        Line(start - tangent * radius, end - tangent * radius, color);
        Line(start + bitangent * radius, end + bitangent * radius, color);
        Line(start - bitangent * radius, end - bitangent * radius, color);

        DrawCircle(start, tangent, bitangent, radius, color, segments);
        
        HemiCircle(start, -tangent, bitangent, radius, color, segments);
        HemiCircle(start, -bitangent, -tangent, radius, color, segments);

        DrawCircle(end, tangent, bitangent, radius, color, segments);
        
        HemiCircle(end, -tangent, -bitangent, radius, color, segments);
        HemiCircle(end, -bitangent, tangent, radius, color, segments);
    }


    
    public static void Sphere(
        Vector3 center,
        float radius,
        Vector4 color,
        int segments = 24) {

        DrawCircle(center, Vector3.UnitX, Vector3.UnitY, radius, color, segments);
        DrawCircle(center, Vector3.UnitY, Vector3.UnitZ, radius, color, segments);
        DrawCircle(center, Vector3.UnitZ, Vector3.UnitX, radius, color, segments);
    }

    public static void Arrow(
        Vector3 start,
        Vector3 end,
        Vector4 color,
        float headLengthRatio = 0.2f,
        float headRadiusRatio = 0.08f,
        int segments = 8) {
        Vector3 dir = end - start;
        float length = dir.Length;
        
        if (length < 0.0001f)
            return;

        dir /= length;

        // Shaft
        float headLength = length * headLengthRatio;
        Vector3 shaftEnd = end - dir * headLength;

        Line(start, shaftEnd, color);

        // Build stable basis
        Vector3 tangent = Vector3.Cross(dir, Vector3.UnitZ);

        if (tangent.LengthSquared < 0.0001f)
            tangent = Vector3.Cross(dir, Vector3.UnitX);
        tangent.Normalize();

        Vector3 bitangent = Vector3.Cross(dir, tangent);

        float headRadius = length * headRadiusRatio;
        Vector3 baseCenter = shaftEnd;

        // Base circle of cone
        DrawCircle(baseCenter, tangent, bitangent, headRadius, color, segments);

        // Connect cone edges
        float angleStep = MathF.Tau / segments;

        for (int i = 0; i < segments; i++) {
            float a = i * angleStep;

            Vector3 offset =
                (tangent * MathF.Cos(a) +
                 bitangent * MathF.Sin(a)) * headRadius;

            Line(baseCenter + offset, end, color);
            Line(baseCenter + offset, shaftEnd, color);
        }
    }
    
    public static void Grid(
        int halfSize = 10,
        float cellSize = 1.0f,
        int majorLineEvery = 5,
        float y = 0.0f)
    {
        Vector4 minorColor = new Vector4(0.25f, 0.25f, 0.25f, 0.5f);
        Vector4 majorColor = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);

        Vector4 xAxis = new Vector4(1, 0, 0, 0.5f);
        Vector4 yAxis = new Vector4(0, 1, 0, 0.5f);
        Vector4 zAxis = new Vector4(0, 0, 1, 0.5f);

        float extent = halfSize * cellSize;

        for (int i = -halfSize; i <= halfSize; i++)
        {
            float pos = i * cellSize;

            bool isMajor = (i % majorLineEvery) == 0;

            Vector4 color = isMajor ? majorColor : minorColor;

            // Z lines (parallel to X)
            Vector3 zStart = new Vector3(-extent, y, pos);
            Vector3 zEnd   = new Vector3( extent, y, pos);
            DebugDraw.Line(zStart, zEnd, color);

            // X lines (parallel to Z)
            Vector3 xStart = new Vector3(pos, y, -extent);
            Vector3 xEnd   = new Vector3(pos, y,  extent);
            DebugDraw.Line(xStart, xEnd, color);
        }

        // Axis lines (draw last so they’re clearest)
        DebugDraw.Line(
            new Vector3(-extent, y, 0),
            new Vector3( extent, y, 0),
            xAxis);

        DebugDraw.Line(
            new Vector3(0, y, -extent),
            new Vector3(0, y,  extent),
            zAxis);

        // Optional Y axis
        DebugDraw.Line(
            new Vector3(0, -extent, 0),
            new Vector3(0,  extent, 0),
            yAxis);
    }

}