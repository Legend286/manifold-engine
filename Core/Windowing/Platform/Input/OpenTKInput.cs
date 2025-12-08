using Manifold.Core.InputSystem;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Manifold.Core.Windowing.Platform.Input;

public class OpenTKInput : InputImplementation {

    private readonly GameWindow _window;

    public OpenTKInput(GameWindow window) {
        _window = window;
    }
    protected internal override bool IsKeyDownImpl(Keys key) {
        return _window.KeyboardState.IsKeyDown(key);
    }

    protected internal override bool IsKeyReleasedImpl(Keys key) {
        return _window.KeyboardState.IsKeyReleased(key);
    }

    protected internal override bool IsKeyPressedImpl(Keys key) {
        return _window.KeyboardState.IsKeyPressed(key);
    }

    protected internal override bool IsMouseButtonDownImpl(MouseButton button) {
        return _window.MouseState.IsButtonDown(button);
    }

    protected internal override Vector2 GetMousePositionImpl() {
        return _window.MouseState.Position;
    }
}