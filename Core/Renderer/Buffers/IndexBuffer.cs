using OpenTK.Graphics.OpenGL.Compatibility;

namespace Manifold.Core.Renderer.Buffers;

public class IndexBuffer : IDisposable {
    private int _rendererID;

    public int Count { get; private set; }

    public IndexBuffer(uint[] indices) {
        _rendererID = GL.GenBuffer();
        Count = indices.Length;

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rendererID);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsage.StaticDraw);
    }

    public void Bind() {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rendererID);
    }

    public void Unbind() {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteBuffer(_rendererID);
    }
}