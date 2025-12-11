using OpenTK.Mathematics;
using Manifold.Core.Renderer.Buffers;
using Manifold.Core.Renderer.Maths;

namespace Manifold.Core.Renderer.Modules;

public class Mesh {
    public VertexArray VAO;
    public IndexBuffer IBO;
    public AABB Bounds;

    public int IndexCount => IBO.Count;

    public Mesh(VertexArray vao, IndexBuffer ibo) {
        VAO = vao;
        IBO = ibo;
        Bounds = new AABB(new Vector3(-0.5f,-0.5f,-0.5f), new Vector3(0.5f, 0.5f, 0.5f));
    }

    public static Mesh Cube() {
        var vertexArray = new VertexArray();
        
        float[] vertices =
        {
            // === Back face (-Z) ===
            -0.5f, -0.5f, -0.5f,   0, 0, -1,   0.6f,0.6f,0.6f,1,
            0.5f, -0.5f, -0.5f,   0, 0, -1,   0.6f,0.6f,0.6f,1,
            0.5f,  0.5f, -0.5f,   0, 0, -1,   0.6f,0.6f,0.6f,1,
            -0.5f,  0.5f, -0.5f,   0, 0, -1,   0.6f,0.6f,0.6f,1,

            // === Front face (+Z) ===
            -0.5f, -0.5f,  0.5f,   0, 0, 1,    0.6f,0.6f,0.6f,1,
            0.5f, -0.5f,  0.5f,   0, 0, 1,    0.6f,0.6f,0.6f,1,
            0.5f,  0.5f,  0.5f,   0, 0, 1,    0.6f,0.6f,0.6f,1,
            -0.5f,  0.5f,  0.5f,   0, 0, 1,    0.6f,0.6f,0.6f,1,

            // === Left face (-X) ===
            -0.5f, -0.5f,  0.5f,  -1, 0, 0,    0.6f,0.6f,0.6f,1,
            -0.5f, -0.5f, -0.5f,  -1, 0, 0,    0.6f,0.6f,0.6f,1,
            -0.5f,  0.5f, -0.5f,  -1, 0, 0,    0.6f,0.6f,0.6f,1,
            -0.5f,  0.5f,  0.5f,  -1, 0, 0,    0.6f,0.6f,0.6f,1,

            // === Right face (+X) === 
            0.5f, -0.5f, -0.5f,   1, 0, 0,    0.6f,0.6f,0.6f,1,
            0.5f, -0.5f,  0.5f,   1, 0, 0,    0.6f,0.6f,0.6f,1,
            0.5f,  0.5f,  0.5f,   1, 0, 0,    0.6f,0.6f,0.6f,1,
            0.5f,  0.5f, -0.5f,   1, 0, 0,    0.6f,0.6f,0.6f,1,

            // === Bottom face (-Y) ===
            -0.5f, -0.5f, -0.5f,   0, -1, 0,   0.6f,0.6f,0.6f,1,
            0.5f, -0.5f, -0.5f,   0, -1, 0,   0.6f,0.6f,0.6f,1,
            0.5f, -0.5f,  0.5f,   0, -1, 0,   0.6f,0.6f,0.6f,1,
            -0.5f, -0.5f,  0.5f,   0, -1, 0,   0.6f,0.6f,0.6f,1,

            // === Top face (+Y) ===
            -0.5f,  0.5f,  0.5f,   0, 1, 0,    0.6f,0.6f,0.6f,1,
            0.5f,  0.5f,  0.5f,   0, 1, 0,    0.6f,0.6f,0.6f,1,
            0.5f,  0.5f, -0.5f,   0, 1, 0,    0.6f,0.6f,0.6f,1,
            -0.5f,  0.5f, -0.5f,   0, 1, 0,    0.6f,0.6f,0.6f,1,
        };


        
        var vertexBuffer = new VertexBuffer(vertices);
        

        var layout = new BufferLayout(
            new BufferElement(ShaderDataType.Float3, "a_Position"),
            new BufferElement(ShaderDataType.Float3, "a_Normal"),
            new BufferElement(ShaderDataType.Float4, "a_Color")
        );
        vertexBuffer.SetLayout(layout);

        vertexArray.AddVertexBuffer(vertexBuffer);
        uint[] indices =
        {
            // Back (-Z) FIXED
            0, 2, 1,
            0, 3, 2,

            // Front (+Z) OK
            4, 5, 6,
            4, 6, 7,

            // Left (-X) FIXED
            8,10, 9,
            8,11,10,

            // Right (+X) OK
            12,14,13,
            12,15,14,

            // Bottom (-Y) FIXED
            16,17,18,
            16,18,19,

            // Top (+Y) OK
            20,21,22,
            20,22,23
        };



        var indexBuffer = new IndexBuffer(indices);

        vertexArray.SetIndexBuffer(indexBuffer);

        return new Mesh(vertexArray, indexBuffer);
    }
}