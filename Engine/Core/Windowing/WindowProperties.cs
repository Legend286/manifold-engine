namespace Manifold.Core.Windowing;

public struct WindowProperties {
    public string Title;
    public int Width;
    public int Height;
    public bool VSync;

    public WindowProperties(string title = "Engine Engine", int width = 1280, int height = 720, bool vsync = true) {
        Title = title;
        Width = width;
        Height = height;
        VSync = vsync;
    }
}