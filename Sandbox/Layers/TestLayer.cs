using System.Net.Mime;
using Manifold.Core.Layers;
using Manifold.Core.Renderer;
using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Renderer.Shaders;
using Manifold.Core.SceneSystem;
using Manifold.Internal;
using Manifold.Runtime;
using OpenTK.Graphics.OpenGL.Compatibility;
using OpenTK.Windowing.Common;

namespace Manifold.Sandbox.Layers;

public class TestLayer : Layer {
    private VertexArray _vertexArray;
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private ShaderProgram _shader;
    private RenderTarget _target;
    public RenderTarget Target => _target;

    private int _shaderProgram;

    public TestLayer() : base("Test Layer") {
        
    }

    public override void OnDisable() {
        base.OnDisable();
        Target.Bind();
        RenderCommand.Clear();
        Target.Unbind();
    }

    public override void OnAttach() {
        Application.Instance.OpenTKWindow.Resize += (args) => _target.Resize(args.Width, args.Height);
        RenderTargetSpec spec = new RenderTargetSpec();
        spec.Width = (int)(Application.Instance.Width * DPI.X);
        spec.Height = (int) (Application.Instance.Height * DPI.Y);
        spec.ColorAttachments = new[] { new RenderTargetAttachmentSpec() { Format = RenderTargetFormat.RGBA8 } };
        spec.HasDepth = true;
        _target = new RenderTarget(spec);
        
        _shader = ShaderManager.Load("DebugModel");
    }
    
    public override void OnRender() {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Disable(EnableCap.Blend);
        GL.DepthMask(true);
        
        _target.Bind(true);
        _shader.Bind();
        _shader.Set("u_View", Application.Instance.MainCamera.GetView());
        _shader.Set("u_Projection", Application.Instance.MainCamera.GetProjection((float)Application.Instance.Width / Application.Instance.Height));
        
        _vertexArray.Bind();

        GL.DrawElements(PrimitiveType.Triangles, _vertexArray.GetIndexBuffer.Count, DrawElementsType.UnsignedInt, 0);
        _target.Unbind();
    }
    
}