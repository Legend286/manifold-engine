using Manifold.Core.Events;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Manifold.Core.Windowing.Platform;

public class OpenTKWindow : GameWindow, IWindow {
   
   
    public int Width => base.Size.X;
    public int Height => base.Size.Y;
    public new string Title => base.Title;
    public bool IsVSyncEnabled => base.VSync == VSyncMode.On;
    private Action<Event> _eventCallback;
    public OpenTKWindow(WindowProperties properties) : base(GameWindowSettings.Default, new NativeWindowSettings() {
        Title = properties.Title,
        ClientSize = new Vector2i(properties.Width, properties.Height),
        Vsync = properties.VSync ? VSyncMode.On : VSyncMode.Off,
        APIVersion = new Version(4, 6),

    }) {
        SetVSync(properties.VSync);
    }

    public void SetEventCallback(Action<Event> callback) {
        _eventCallback = callback;
    }
    public void SetVSync(bool enabled) {
        base.VSync = enabled ? VSyncMode.On : VSyncMode.Off;
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        var props = new WindowResizeEvent(Size.X, Size.Y);
        _eventCallback?.Invoke(props);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
        base.OnClosing(e);
        var closeEvent = new WindowCloseEvent();
        _eventCallback?.Invoke(closeEvent);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        var keyEvent = new KeyPressedEvent(e.Key, 0);
        _eventCallback?.Invoke(keyEvent);
    }
    
    public void OnUpdate() {
        ProcessEvents(1);
        SwapBuffers();
    }
    
}