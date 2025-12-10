using Manifold.Runtime;
using OpenTK.Mathematics;

namespace Manifold.Core.SceneSystem;

public class Camera {
    public float Fov = 90.0f;
    public float Near = 0.1f;
    public float Far = 1000f;

    public Camera() {
        Application.Instance.SetCamera(this);
    }
    public Transform Transform { get; set; } = new Transform();

    public Matrix4 GetProjection(float aspectRatio) {
        return Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(Fov),
            aspectRatio,
            Near,
            Far
        );
    }

    public Matrix4 GetView() {
        return Transform.GetViewMatrix();
    }
}