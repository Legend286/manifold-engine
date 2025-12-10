using System.Net.Mime;
using Manifold.Core.Layers;
using Manifold.Core.Renderer;
using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Renderer.Shaders;
using Manifold.Core.SceneSystem;
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
        spec.Width = Application.Instance.Width;
        spec.Height = Application.Instance.Height;
        spec.ColorAttachments = new[] { new RenderTargetAttachmentSpec() { Format = RenderTargetFormat.RGBA8 } };
        spec.HasDepth = true;
        _target = new RenderTarget(spec);
        _vertexArray = new VertexArray();
        float[] vertices =
        {
            // Positions              // Colors
            -0.5f, -0.5f, -0.5f,      0.5f, 0.5f, 0.5f, 1, // 0
            0.5f, -0.5f, -0.5f,      0.5f, 0.5f, 0.5f, 1, // 1
            0.5f,  0.5f, -0.5f,      0.5f, 0.5f, 0.5f, 1, // 2
            -0.5f,  0.5f, -0.5f,      0.5f, 0.5f, 0.5f, 1, // 3

            -0.5f, -0.5f,  0.5f,      0.5f, 0.5f, 0.5f, 1, // 4
            0.5f, -0.5f,  0.5f,      0.5f, 0.5f, 0.5f, 1, // 5
            0.5f,  0.5f,  0.5f,      0.5f, 0.5f, 0.5f, 1, // 6
            -0.5f,  0.5f,  0.5f,      0.5f, 0.5f, 0.5f, 1  // 7
        };

        
        _vertexBuffer = new VertexBuffer(vertices);
        

        var layout = new BufferLayout(
            new BufferElement(ShaderDataType.Float3, "a_Position"),
            new BufferElement(ShaderDataType.Float4, "a_Color")
        );
        _vertexBuffer.SetLayout(layout);

        _vertexArray.AddVertexBuffer(_vertexBuffer);

        uint[] indices =
        {
            // Back (-Z)
            0, 2, 1,
            0, 3, 2,

            // Front (+Z)
            4, 5, 6,
            4, 6, 7,

            // Left (-X)
            0, 7, 3,
            0, 4, 7,

            // Right (+X)
            1, 2, 6,
            1, 6, 5,

            // Bottom (-Y)
            0, 1, 5,
            0, 5, 4,

            // Top (+Y)
            3, 6, 2,
            3, 7, 6
        };

        _indexBuffer = new IndexBuffer(indices);

        _vertexArray.SetIndexBuffer(_indexBuffer);
        
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