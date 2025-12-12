using System.Collections;

namespace Manifold.Core.Renderer.Buffers;

public enum ShaderDataType {
    None = 0, Float, Float2, Float3, Float4, Mat3, Mat4, Int, Int2, Int3, Int4, Bool,
}

static class ShaderDataTypeExtensions {
    public static int Size(this ShaderDataType type) {
        switch (type) {
            case ShaderDataType.Float: return 4;
            case ShaderDataType.Float2: return 4 * 2;
            case ShaderDataType.Float3: return 4 * 3;
            case ShaderDataType.Float4: return 4 * 4;
            case ShaderDataType.Mat3: return 4 * 3 * 3;
            case ShaderDataType.Mat4: return 4 * 4 * 4;
            case ShaderDataType.Int: return 4;
            case ShaderDataType.Int2: return 4 * 2;
            case ShaderDataType.Int3: return 4 * 3;
            case ShaderDataType.Int4: return 4 * 4;
            case ShaderDataType.Bool: return 4;
        }

        return 0;
    }
}

public struct BufferElement {
    public string Name;
    public ShaderDataType Type;
    public int Size;
    public int Offset;
    public bool Normalized;

    public BufferElement(ShaderDataType type, string name, bool normalized = false) {
        Name = name;
        Type = type;
        Size = type.Size();
        Offset = 0;
        Normalized = normalized;
    }

    public int GetComponentCount() {
        switch (Type) {
            case ShaderDataType.Float: return 1;
            case ShaderDataType.Float2: return 2;
            case ShaderDataType.Float3: return 3;
            case ShaderDataType.Float4: return 4;
            case ShaderDataType.Mat3: return 3 * 3;
            case ShaderDataType.Mat4: return 4 * 4;
            case ShaderDataType.Int: return 1;
            case ShaderDataType.Int2: return 2;
            case ShaderDataType.Int3: return 3;
            case ShaderDataType.Int4: return 4;
            case ShaderDataType.Bool: return 1;
        }

        return 0;
    }
}

public class BufferLayout : IEnumerable<BufferElement> {
    private readonly List<BufferElement> _elements;

    public int Stride { get; private set; }

    public BufferLayout(params BufferElement[] elements) {
        _elements = new List<BufferElement>(elements);
        CalculateOffsetsAndStride();
    }

    public IReadOnlyList<BufferElement> Elements => _elements;

    private void CalculateOffsetsAndStride() {
        int offset = 0;
        Stride = 0;

        for (int i = 0; i < _elements.Count; i++) {
            var element = _elements[i];
            element.Offset = offset;
            _elements[i] = element;

            offset += element.Size;
            Stride += element.Size;
        }
    }

    public IEnumerator<BufferElement> GetEnumerator() => _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}