using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Manifold.Core.Layers;
using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.Shaders;
using OpenTK.Graphics.OpenGLES2;

namespace Manifold.Core.Renderer.Debug.Layers;

public class DebugDrawLayer : Layer
{
    private VertexArray _vao;
    private VertexBuffer _vbo;
    private ShaderProgram _shader;

    private const int MaxVertices = 250_000;
    
    public DebugDrawLayer() : base("Debug Draw Layer") {
            
    }

    public override void OnAttach()
    {
        _vao = new VertexArray();

        _vbo = new VertexBuffer(MaxVertices * Unsafe.SizeOf<DebugVertex>());
        _vbo.SetLayout(DebugVertex.Layout);

        _vao.AddVertexBuffer(_vbo);

        _shader = ShaderManager.Load("debug");
    }

    public override void OnRender()
    {
        var verts = DebugDraw.Vertices;
        if (verts.Count == 0)
            return;

        _vbo.Bind();
        GL.BufferSubData(
            BufferTarget.ArrayBuffer,
            IntPtr.Zero,
            verts.Count * Unsafe.SizeOf<DebugVertex>(),
            verts.ToArray());
        _shader.Bind();
        _vao.Bind();
        
        GL.DrawArrays(PrimitiveType.Lines, 0, verts.Count);
        
        DebugDraw.Clear();
    }
}