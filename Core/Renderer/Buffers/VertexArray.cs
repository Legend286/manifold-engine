using Manifold.Core.Renderer.Buffers;
using OpenTK.Graphics.OpenGL.Compatibility;

namespace Manifold.Core.Renderer.Buffers;

public class VertexArray : IDisposable
{
    private int _rendererID;
    private int _vertexBufferIndex = 0;
    private List<VertexBuffer> _vertexBuffers = new List<VertexBuffer>();
    private IndexBuffer _indexBuffer;

    public VertexArray()
    {
        _rendererID = GL.GenVertexArray();
    }

    public void Bind()
    {
        GL.BindVertexArray(_rendererID);
    }

    public void Unbind()
    {
        GL.BindVertexArray(0);
    }

    public void AddVertexBuffer(VertexBuffer vertexBuffer)
    {
        if (vertexBuffer.GetLayout().Elements.Count == 0)
        {
            throw new Exception("Vertex buffer has no layout.");
        }

        Bind();
        vertexBuffer.Bind();

        var layout = vertexBuffer.GetLayout();

        foreach (var element in layout.Elements)
        {
            switch (element.Type)
            {
                // Matrices must be split into multiple Vec4 attributes
                case ShaderDataType.Mat3:
                case ShaderDataType.Mat4:
                {
                    int count = element.GetComponentCount(); // e.g., 16 for Mat4
                    int countVec4 = count / 4; // e.g., 4 attributes needed
                    int sizeVec4 = 4; // Each attribute is a vec4
                    int strideVec4 = sizeVec4 * 4; // 16 bytes per row

                    for (int i = 0; i < countVec4; i++)
                    {
                        GL.EnableVertexAttribArray((uint)_vertexBufferIndex);
                        GL.VertexAttribPointer(
                            (uint)_vertexBufferIndex,
                            sizeVec4,
                            ShaderDataTypeToOpenGLBaseType(element.Type),
                            element.Normalized,
                            layout.Stride,
                            (IntPtr)(element.Offset + (sizeof(float) * sizeVec4 * i)) // Offset pointer
                        );
                        // Make the divisor 1 so this attributes updates per-instance (if using instancing)
                        // GL.VertexAttribDivisor(_vertexBufferIndex, 1); 
                        _vertexBufferIndex++;
                    }
                    break;
                }
                
                // Integers must use IPointer to avoid Float conversion
                case ShaderDataType.Int:
                case ShaderDataType.Int2:
                case ShaderDataType.Int3:
                case ShaderDataType.Int4:
                {
                    GL.EnableVertexAttribArray((uint)_vertexBufferIndex);
                    GL.VertexAttribIPointer(
                        (uint)_vertexBufferIndex,
                        element.GetComponentCount(),
                        (VertexAttribIType)ShaderDataTypeToOpenGLBaseType(element.Type),
                        layout.Stride,
                        (IntPtr)element.Offset
                    );
                    _vertexBufferIndex++;
                    break;
                }
                
                // Standard Floats/Vectors
                default:
                {
                    GL.EnableVertexAttribArray((uint)_vertexBufferIndex);
                    GL.VertexAttribPointer(
                        (uint)_vertexBufferIndex,
                        element.GetComponentCount(),
                        ShaderDataTypeToOpenGLBaseType(element.Type),
                        element.Normalized,
                        layout.Stride,
                        (IntPtr)element.Offset // Explicit cast to IntPtr is safer in OpenTK
                    );
                    _vertexBufferIndex++;
                    break;
                }
            }
        }

        _vertexBuffers.Add(vertexBuffer);
    }

    public void SetIndexBuffer(IndexBuffer indexBuffer)
    {
        Bind();
        indexBuffer.Bind();
        _indexBuffer = indexBuffer;
    }

    public IndexBuffer GetIndexBuffer => _indexBuffer;

    public void Dispose()
    {
        GL.DeleteVertexArray(_rendererID);
    }

    private static VertexAttribPointerType ShaderDataTypeToOpenGLBaseType(ShaderDataType type)
    {
        switch (type)
        {
            case ShaderDataType.Float:
            case ShaderDataType.Float2:
            case ShaderDataType.Float3:
            case ShaderDataType.Float4:
            case ShaderDataType.Mat3:
            case ShaderDataType.Mat4:
                return VertexAttribPointerType.Float;
            case ShaderDataType.Int:
            case ShaderDataType.Int2:
            case ShaderDataType.Int3:
            case ShaderDataType.Int4:
                return VertexAttribPointerType.Int;
            case ShaderDataType.Bool:
                return VertexAttribPointerType.UnsignedByte; // Usually safer for bools
        }
        return 0;
    }
}