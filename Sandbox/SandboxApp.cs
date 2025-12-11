using Manifold.Core.InputSystem;
using Manifold.Core.Renderer.Containers;
using Manifold.Core.Renderer.Debug;
using Manifold.Core.Renderer.Debug.Layers;
using Manifold.Core.Renderer.Layers;
using Manifold.Core.Renderer.MaterialSystem;
using Manifold.Core.Renderer.Modules;
using Manifold.Core.Renderer.Shaders;
using Manifold.Core.SceneSystem;
using Manifold.Internal;
using Manifold.Runtime;
using Manifold.Sandbox.Layers;
using OpenTK.Graphics.OpenGLES2;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sandbox;

public class SandboxApp : Application {
   // private TestLayer TL;
    private Camera _camera;
    private FlyCameraController _controller;
    private Transform t;
    private Scene scene;
    public SandboxApp(string title) : base(title) {
        _camera = new Camera();
        _controller = new FlyCameraController(_camera);
        t = new Transform() { Position = new Vector3(0, 2, 0) };
        RenderTargetSpec spec = new RenderTargetSpec() {
            ColorAttachments = new[] { new RenderTargetAttachmentSpec() { Format = RenderTargetFormat.RGBA8 } },
            HasDepth = true,
            Width = Application.Instance.Width,
            Height = Application.Instance.Height,
        };
        scene = new Scene();
        
        var target = new RenderTarget(spec);
        Application.Instance.OpenTKWindow.Resize += (ResizeEventArgs e) => target.Resize(e.Width, e.Height);

        for (int i = -100; i <= 100; i++) {
            for (int j = -100; j <= 100; j++) {
                scene.AddRenderable(new Renderable(Mesh.Cube(), new Material(ShaderManager.Load("DebugModel")), new Transform() { Position = new Vector3(i*2, 0, j*2)}));
            }
        }
        scene.AddRenderable(new Renderable(Mesh.Cube(), new Material(ShaderManager.Load("DebugModel")), t));
       
        var forwardLayer = new ForwardRenderLayer(target, scene);
        PushLayer(forwardLayer);
      
        PushLayer(new FullscreenBlitLayer(target));
        PushLayer(new DebugDrawLayer(target));
        
        PushOverlay(new ImGuiLayer());
    }

    private float dtAccum = 0;
    protected override void OnUpdate(float deltaTime) {
        base.OnUpdate(deltaTime);

        dtAccum += deltaTime;
        t.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, dtAccum);
        t.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitX, dtAccum);
        if (Input.IsKeyDown(Keys.Escape)) {
            this.Close();
        }
        
        if (Input.IsKeyPressed(Keys.F1)) {
            LayerStack.First().IsVisible = !LayerStack.First().IsVisible;
        }
        
        DebugDraw.AABB(scene.Renderables.Last().GetRenderBounds(), new Vector4(1,0,1,1));
        
        _controller.Update(deltaTime);
        
        GL.LineWidth(8.0f);
        DebugDraw.Line(new Vector3(1,0,0), new Vector3(2,0,0), new Vector4(1, 0, 0, 1));
        
        DebugDraw.AABB(new Vector3(4,0,0), Vector3.One * 5, new Vector4(0, 1, 0, 1));

        DebugDraw.Transform(new Vector3(3, 0, 0), Quaternion.Identity, 0.5f);
        
        DebugDraw.Point(new Vector3(1,0,1), 0.5f, new Vector4(0,0, 1, 1));
        
        DebugDraw.Capsule(new Vector3(1,0,3), new Vector3(1,1,3), 0.5f, new Vector4(1,0,1,1));

        DebugDraw.Arrow(new Vector3(2, 0, 2), new Vector3(3, 1, -1), new Vector4(1, 1, 0, 1));
       
        DebugDraw.Grid(50, 1.0f, 5, 0.0f);
        
        DebugDraw.Sphere(new Vector3(6,0,1), 0.5f, new Vector4(1, 1, 0, 1));
    }
    
    protected override void OnRender() {
        base.OnRender();
        
    }
    
}