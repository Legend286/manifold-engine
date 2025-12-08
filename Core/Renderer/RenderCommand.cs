using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Mathematics;

namespace Manifold.Core.Renderer;

public static class RenderCommand {
    public static void Init() {
        GL.Enable(EnableCap.DepthTest);
    }

    public static void SetClearColor(Color4<Rgba> color) {
        GL.ClearColor(color);
    }

    public static void Clear() {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
}