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

    public Matrix4 GetProjection(float aspectRatio, float fovExpand = 1.0f) {
        return Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(Fov * fovExpand),
            aspectRatio,
            Near,
            Far
        );
    }

    public Matrix4 GetView() {
        return Transform.GetViewMatrix();
    }

    public Matrix4 GetViewProjection() {
        return GetView() * GetProjection((float)Application.Instance.Width / Application.Instance.Height);
    }

    public Matrix4 GetViewProjectionCulling(float padMul = 1.25f) {
        return Matrix4.Transpose(GetView() * GetProjection((float)Application.Instance.Width / Application.Instance.Height, padMul));
    }
}