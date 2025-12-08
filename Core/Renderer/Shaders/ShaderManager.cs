using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Manifold.Core.Renderer.Shaders;

public static class ShaderManager {
    public static string ShaderDirectory = "Assets/Shaders";

    private static readonly Dictionary<string, ShaderProgram> _shaders = new();

    /// <summary>
    /// Load a shader program by name.
    /// Expects {name}.vert and {name}.frag in Assets/Shaders.
    /// </summary>
    public static ShaderProgram Load(string name) {
        if (_shaders.TryGetValue(name, out var shader))
            return shader;

        string vertPath = Path.Combine(ShaderDirectory, name + ".vert");
        string fragPath = Path.Combine(ShaderDirectory, name + ".frag");

        if (!File.Exists(vertPath))
            throw new FileNotFoundException($"Vertex shader not found: {vertPath}");

        if (!File.Exists(fragPath))
            throw new FileNotFoundException($"Fragment shader not found: {fragPath}");

        string vertexSource = LoadSource(vertPath);
        string fragmentSource = LoadSource(fragPath);

        int program = CompileProgram(name, vertexSource, fragmentSource);

        shader = new ShaderProgram(name, program);
        _shaders[name] = shader;

        return shader;
    }

    public static ShaderProgram Get(string name)
        => _shaders[name];

    public static void DisposeAll() {
        foreach (var shader in _shaders.Values)
            shader.Dispose();

        _shaders.Clear();
    }

    private static int CompileProgram(
        string name,
        string vertexSource,
        string fragmentSource) {
        int program = GL.CreateProgram();

        int vs = CompileShader(name, ShaderType.VertexShader, vertexSource);
        int fs = CompileShader(name, ShaderType.FragmentShader, fragmentSource);

        GL.AttachShader(program, vs);
        GL.AttachShader(program, fs);
        GL.LinkProgram(program);

        GL.GetProgrami(program, ProgramProperty.LinkStatus, out int status);

        if (status == 0) {
            string log = GL.GetProgramInfoLog(program, 1024, out int length);

            throw new Exception($"[Shader:{name}] Link error:\n{log}");
        }


        GL.DetachShader(program, vs);
        GL.DetachShader(program, fs);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);

        return program;
    }

    private static int CompileShader(string name, ShaderType type, string source) {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShaderi(shader, ShaderParameterName.CompileStatus, out int status);

        if (status == 0) {
            string log = GL.GetShaderInfoLog(shader, 1024, out int length);

            throw new Exception($"[Shader:{name}] {type} compile error:\n{log}");
        }


        return shader;
    }

    // =============================
    // File loading + includes
    // =============================

    private static string LoadSource(string path) {
        string dir = Path.GetDirectoryName(path)!;
        string source = File.ReadAllText(path);

        return ProcessIncludes(source, dir);
    }

    private static string ProcessIncludes(string source, string baseDir) {
        var sb = new StringBuilder();

        foreach (var line in source.Split('\n')) {
            string trimmed = line.TrimStart();

            if (trimmed.StartsWith("#include")) {
                int start = line.IndexOf('"') + 1;
                int end = line.LastIndexOf('"');

                if (start <= 0 || end <= start)
                    throw new Exception($"Invalid #include syntax: {line}");

                string includeFile = line[start..end];
                string includePath = Path.Combine(baseDir, includeFile);

                if (!File.Exists(includePath))
                    throw new FileNotFoundException($"Included file not found: {includePath}");

                sb.AppendLine(ProcessIncludes(
                    File.ReadAllText(includePath),
                    baseDir
                ));
            }
            else {
                sb.AppendLine(line);
            }
        }


        return sb.ToString();
    }
}