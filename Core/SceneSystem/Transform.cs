using OpenTK.Mathematics;

namespace Manifold.Core.SceneSystem;

public class Transform {
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;

    public Matrix4 GetLocalMatrix() {
        return
            Matrix4.CreateScale(Scale) *
            Matrix4.CreateFromQuaternion(Rotation) *
            Matrix4.CreateTranslation(Position);
    }

    public Matrix4 GetViewMatrix() {
        // View matrix is inverse of transform
        Matrix4.Invert(GetLocalMatrix(), out var view);
        return view;
    }

    public Vector3 Forward =>
        Vector3.Transform(-Vector3.UnitZ, Rotation);

    public Vector3 Up =>
        Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Right =>
        Vector3.Transform(Vector3.UnitX, Rotation);
}