using Manifold.Core.Renderer.Shaders;

namespace Manifold.Core.Renderer.MaterialSystem;

public class Material {
    public ShaderProgram Shader;

    public Material(ShaderProgram shader) {
        Shader = shader;
    }
    public void Apply() {
        // textures
        // uniforms
    }
}
