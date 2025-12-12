using Manifold.Core.Layers;
using Manifold.Core.Renderer.Containers;
using Manifold.Core.Renderer.Culling;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.SceneSystem;
using Manifold.Core.Renderer.Modules;
using Manifold.Core.Runtime;
using Manifold.Internal;
using OpenTK.Graphics.OpenGL;

namespace Manifold.Core.Renderer.Layers;

public class ForwardRenderLayer : Layer {
    private RenderTarget _target;
    private Scene _scene;
    private RenderContext _context;

    public ForwardRenderLayer(RenderTarget target, Scene scene)
        : base("Forward Renderer") {
        _target = target;
        _scene = scene;
        _context = new RenderContext();
    }

    public override void OnDisable() {
        base.OnDisable();
        _target.Bind();
        RenderCommand.Clear();
        _target.Unbind();
    }

    public override void OnUpdate(float deltaTime) {
    }

    public override void OnRender() {

        _context.Cull(_scene, Application.Instance.MainCamera);
       
        _context.WaitForCulling(3); // Wait AFTER rendering
        
        // Render immediately using the last fully completed visibility set
        _target.Bind(clear: true);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        ForwardRenderer.BeginScene(Application.Instance.MainCamera);

        foreach (int id in _context.VisibleRenderables) {
            Renderable r = _scene.Renderables[id];
            Counter.MarkRenderableVisible();

            ForwardRenderer.Submit(
                r.Mesh,
                r.Material,
                r.Transform.GetLocalMatrix());
        }


        ForwardRenderer.EndScene();
        _target.Unbind();
        

       
    }
}