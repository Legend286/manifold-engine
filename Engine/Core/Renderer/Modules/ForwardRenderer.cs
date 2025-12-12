using System.Collections.Generic;
using Manifold.Core.Renderer.Containers;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Runtime;
using Manifold.Core.SceneSystem;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Manifold.Core.Renderer.Modules;

public static class ForwardRenderer {
    private static Matrix4 _view;
    private static Matrix4 _projection;
    
    public static void BeginScene(Camera camera) {
        float aspect = (float)Application.Instance.Width / Application.Instance.Height;

        _view = camera.GetView();
        _projection = camera.GetProjection(aspect);
    }

    public static void Submit(Mesh mesh, Material material, Matrix4 transform) {
        // Bind shader + upload matrices
        material.Shader.Bind();
        material.Shader.Set("u_View", _view);
        material.Shader.Set("u_Projection", _projection);
        material.Shader.Set("u_Model", transform);

        // Bind material textures & params
        material.Apply();

        // Draw normally
        mesh.VAO.Bind();

        GL.DrawElements(
            PrimitiveType.Triangles,
            mesh.IndexCount,
            DrawElementsType.UnsignedInt,
            IntPtr.Zero);
    }

    public static void EndScene() {
    }
}