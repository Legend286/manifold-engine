using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Manifold.Core.Layers;
using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Renderer.Shaders;
using Manifold.Core.Runtime;
using Manifold.Internal;
using OpenTK.Graphics.OpenGLES2;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Manifold.Core.Renderer.Debug.Layers;

public class DebugDrawLayer : Layer
{
    private VertexArray _vao;
    private VertexBuffer _vbo;
    private ShaderProgram _shader;
    private RenderTarget _target;

    private const int MaxVertices = 250_000;
    
    public DebugDrawLayer(RenderTarget target) : base("Debug Draw Layer") {
        _target = target;
    }

    public override void OnAttach()
    {
        _vao = new VertexArray();

        _vbo = new VertexBuffer(MaxVertices * Unsafe.SizeOf<DebugVertex>());
        _vbo.SetLayout(DebugVertex.Layout);

        _vao.AddVertexBuffer(_vbo);

        _shader = ShaderManager.Load("DebugVis");
    }

    public override void OnRender()
    {
        var verts = DebugDraw.Vertices;
        if (verts.Count == 0)
            return;
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, (int)(Application.Instance.Width * DPI.X), (int)(Application.Instance.Height * DPI.Y));
        
        GL.Disable(EnableCap.DepthTest);
        GL.DepthMask(false);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _vbo.Bind();
        GL.BufferSubData(
            BufferTarget.ArrayBuffer,
            IntPtr.Zero,
            verts.Count * Unsafe.SizeOf<DebugVertex>(),
            verts.ToArray()
        );

        _shader.Bind();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2d, _target.GetDepthTexture());
        _shader.Set("u_Depth", 0);
        _shader.Set("u_View", Application.Instance.MainCamera.GetView());

        _shader.Set("u_Projection",
            Application.Instance.MainCamera.GetProjection(
                (float)Application.Instance.Width /
                Application.Instance.Height
            ));
        
        _vao.Bind();
        GL.DrawArrays(PrimitiveType.Lines, 0, verts.Count);

        DebugDraw.Clear();

        // Restore safety
        GL.Disable(EnableCap.Blend);
        GL.DepthMask(true);
        GL.Enable(EnableCap.DepthTest);
    }
}