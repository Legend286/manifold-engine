using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Manifold.Core.InputSystem;

namespace Manifold.Core.SceneSystem;

public class FlyCameraController {
    private readonly Camera _camera;

    public float MoveSpeed = 6.0f;
    public float MouseSensitivity = 0.0025f;
    public float BoostMultiplier = 3.0f;

    private bool _firstMouse = true;
    private Vector2 _lastMousePos;

    private float _yaw;   // radians
    private float _pitch; // radians

    public FlyCameraController(Camera camera) {
        _camera = camera;

        // Initialize yaw/pitch from existing rotation
        var forward = camera.Transform.Forward;

        _yaw = MathF.Atan2(forward.X, -forward.Z);
        _pitch = MathF.Asin(forward.Y);
    }

    public void Update(float deltaTime) {
        HandleMovement(deltaTime);
        HandleMouseLook();
       
    }

    private void HandleMouseLook() {
        if (!Input.IsMouseButtonDown(MouseButton.Right)) {
            _firstMouse = true;
            return;
        }


        Vector2 mousePos = new Vector2(Input.GetMouseX(), Input.GetMouseY());

        if (_firstMouse) {
            _lastMousePos = mousePos;
            _firstMouse = false;
            return;
        }

        Vector2 delta = mousePos - _lastMousePos;
        _lastMousePos = mousePos;

        _yaw   -= delta.X * MouseSensitivity;
        _pitch -= delta.Y * MouseSensitivity;

        // Clamp pitch to avoid flipping
        _pitch = Math.Clamp(
            _pitch,
            MathHelper.DegreesToRadians(-89.9f),
            MathHelper.DegreesToRadians(89.9f)
        );

        // Build quaternion from yaw (Y) then pitch (X)
        Quaternion yawQ   = Quaternion.FromAxisAngle(Vector3.UnitY, _yaw);
        Quaternion pitchQ = Quaternion.FromAxisAngle(Vector3.UnitX, _pitch);

        _camera.Transform.Rotation = Quaternion.Normalize(yawQ * pitchQ);
    }

    private void HandleMovement(float dt) {
        var t = _camera.Transform;

        Vector3 forward = t.Forward;
        Vector3 right   = t.Right;
        Vector3 up      = t.Up;

        float speed = MoveSpeed;
        if (Input.IsKeyDown(Keys.LeftShift))
            speed *= BoostMultiplier;

        Vector3 move = Vector3.Zero;

        if (Input.IsKeyDown(Keys.W)) move += forward;
        if (Input.IsKeyDown(Keys.S)) move -= forward;
        if (Input.IsKeyDown(Keys.D)) move += right;
        if (Input.IsKeyDown(Keys.A)) move -= right;
        if (Input.IsKeyDown(Keys.E)) move += up;
        if (Input.IsKeyDown(Keys.Q)) move -= up;

        if (move.LengthSquared > 0.0001f) {
            move = Vector3.Normalize(move);
            t.Position += move * speed * dt;
        }
    }
}
