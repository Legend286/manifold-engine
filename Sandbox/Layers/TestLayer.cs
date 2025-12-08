using Manifold.Core.Layers;
using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.Shaders;
using Manifold.Core.SceneSystem;
using Manifold.Runtime;
using OpenTK.Graphics.OpenGL.Compatibility;

namespace Manifold.Sandbox.Layers;

public class TestLayer : Layer {
    private VertexArray _vertexArray;
    private VertexBuffer _vertexBuffer;
    private VertexBuffer _colorBuffer;
    private IndexBuffer _indexBuffer;
    private ShaderProgram _shader;
    private Camera _camera;
    private FlyCameraController _controller;

    private int _shaderProgram;

    public TestLayer() : base("Test Layer") {
        
    }

    public override void OnAttach() {
        _vertexArray = new VertexArray();

        
        _camera = new Camera();
        _controller = new FlyCameraController(_camera);
        
        float[] vertices = {
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f,
        };
        
        _vertexBuffer = new VertexBuffer(vertices);
        

        var layout = new BufferLayout(
            new BufferElement(ShaderDataType.Float3, "a_Position"),
            new BufferElement(ShaderDataType.Float4, "a_Color")
        );
        _vertexBuffer.SetLayout(layout);

        _vertexArray.AddVertexBuffer(_vertexBuffer);

        uint[] indices = { 0, 1, 2 };
        _indexBuffer = new IndexBuffer(indices);

        _vertexArray.SetIndexBuffer(_indexBuffer);
        
        _shader = ShaderManager.Load("debug");
    }

    public override void OnUpdate(float deltaTime) {
        _controller.Update(deltaTime);
    }
    public override void OnRender() {
        
        _shader.Bind();
        _shader.Set("u_View", _camera.GetView());
        _shader.Set("u_Projection", _camera.GetProjection((float)Application.Instance.Width / Application.Instance.Height));
        
        _vertexArray.Bind();

        GL.DrawElements(PrimitiveType.Triangles, _vertexArray.GetIndexBuffer.Count, DrawElementsType.UnsignedInt, 0);
        
    }
    
}