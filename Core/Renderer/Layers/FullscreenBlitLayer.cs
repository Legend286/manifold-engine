using Manifold.Core.Layers;
using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Renderer.Shaders;
using Manifold.Core.SceneSystem;
using Manifold.Runtime;
using OpenTK.Graphics.OpenGL;

namespace Manifold.Core.Renderer.Layers;

public class FullscreenBlitLayer : Layer {

    private VertexArray _vertexArray;
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private ShaderProgram _shader;
    private RenderTarget _source;
    private int _shaderProgram;

    public FullscreenBlitLayer(RenderTarget source) : base("Fullscreen Blit Layer") {
        _source = source;
    }

    public override void OnAttach() {
        _vertexArray = new VertexArray();

        float[] quadVerts = {
            // pos      // uv
            -1f, -1f, 0f, 0f,
            1f, -1f, 1f, 0f,
            1f, 1f, 1f, 1f,
            -1f, 1f, 0f, 1f,
        };

        _vertexBuffer = new VertexBuffer(quadVerts);


        var layout = new BufferLayout(
            new BufferElement(ShaderDataType.Float2, "a_Position"),
            new BufferElement(ShaderDataType.Float2, "a_TexCoord")
        );
        _vertexBuffer.SetLayout(layout);

        _vertexArray.AddVertexBuffer(_vertexBuffer);

        uint[] quadIndices = {
            0, 1, 2,
            2, 3, 0
        };
        _indexBuffer = new IndexBuffer(quadIndices);

        _vertexArray.SetIndexBuffer(_indexBuffer);

        _shader = ShaderManager.Load("Blit");
    }
    
    public override void OnRender() {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.ScissorTest);
        GL.Viewport(0, 0, Application.Instance.Width, Application.Instance.Height);
        
        _shader.Bind();
        
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2d, _source.GetColorTexture());
       
        _shader.Set("u_Texture", 0);
        
        _vertexArray.Bind();
        GL.DrawElements(PrimitiveType.Triangles, _vertexArray.GetIndexBuffer.Count, DrawElementsType.UnsignedInt, 0);
    }
}