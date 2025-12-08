using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Manifold.Core.Input;

public static class Input {
    private static InputImplementation _instance;

    public static void Init(InputImplementation instance) {
        _instance = instance;
    }

    public static bool IsKeyDown(Keys key) {
        return _instance.IsKeyDownImpl(key);
    }

    public static bool IsKeyPressed(Keys key) {
        return _instance.IsKeyPressedImpl(key);
    }

    public static bool IsMouseButtonDown(MouseButton button) {
        return _instance.IsMouseButtonDownImpl(button);
    }

    public static Vector2 GetMousePosition() {
        return _instance.GetMousePositionImpl();
    }

    public static float GetMouseX() => GetMousePosition().X;
    public static float GetMouseY() => GetMousePosition().Y;
}

public abstract class InputImplementation {
    protected internal abstract bool IsKeyDownImpl(Keys key);
    protected internal abstract bool IsKeyPressedImpl(Keys key);
    protected internal abstract bool IsMouseButtonDownImpl(MouseButton button);
    protected internal abstract Vector2 GetMousePositionImpl();
}