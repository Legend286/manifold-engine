using System.Numerics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Manifold.Core.Renderer.Shaders;

public sealed class ShaderProgram : IDisposable
{
    public int Handle { get; }
    public string Name { get; }

    private readonly Dictionary<string, int> _uniformLocations = new();

    internal ShaderProgram(string name, int handle)
    {
        Name = name;
        Handle = handle;
    }

    public void Bind()
    {
        GL.UseProgram(Handle);
    }

    public static void Unbind()
    {
        GL.UseProgram(0);
    }

    public int GetUniformLocation(string name)
    {
        if (_uniformLocations.TryGetValue(name, out var loc))
            return loc;

        loc = GL.GetUniformLocation(Handle, name);

        if (loc == -1)
            Console.WriteLine($"[Shader] Uniform '{name}' not found in '{Name}'");

        _uniformLocations[name] = loc;
        return loc;
    }

    #region Uniform helpers

    public void Set(string name, int value)
        => GL.Uniform1i(GetUniformLocation(name),1, value);

    public void Set(string name, float value)
        => GL.Uniform1f(GetUniformLocation(name),1, value);

    public void Set(string name, Vector2 value)
        => GL.Uniform2f(GetUniformLocation(name),1, value);

    public void Set(string name, Vector3 value)
        => GL.Uniform3f(GetUniformLocation(name),1, value);

    public void Set(string name, Vector4 value)
        => GL.Uniform4f(GetUniformLocation(name),1, value);

    public void Set(string name, Matrix4x4 value)
        => GL.UniformMatrix4f(GetUniformLocation(name),1, false, ref value);

    #endregion

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}
