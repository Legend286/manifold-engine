using OpenTK.Graphics.OpenGL.Compatibility;

namespace Manifold.Core.Renderer.Buffers;

public class VertexBuffer : IDisposable {

    private int _rendererID;
    private BufferLayout _layout;

    public VertexBuffer(float[] vertices) {
        _rendererID = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererID);

        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);
    }

    public VertexBuffer(int size) {
        _rendererID = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererID);
        GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, BufferUsage.DynamicDraw);
    }

    public void Bind() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererID);
    }

    public void Unbind() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void SetLayout(BufferLayout layout) => _layout = layout;
    public BufferLayout GetLayout() => _layout;
    
    
    public void Dispose() {
        GL.DeleteBuffer(_rendererID);
    }
}