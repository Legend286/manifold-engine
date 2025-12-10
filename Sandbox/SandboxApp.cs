using ImGuiNET;
using Manifold.Core.InputSystem;
using Manifold.Core.Layers;
using Manifold.Core.Renderer.Debug;
using Manifold.Core.Renderer.Debug.Layers;
using Manifold.Core.Renderer.Layers;
using Manifold.Core.SceneSystem;
using Manifold.Runtime;
using Manifold.Sandbox.Layers;
using OpenTK.Graphics.OpenGLES2;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sandbox;

public class SandboxApp : Application {
    private TestLayer TL;
    private Camera _camera;
    private FlyCameraController _controller;
    public SandboxApp(string title) : base(title) {
        TL = new TestLayer();
        _camera = new Camera();
        _controller = new FlyCameraController(_camera);
        PushLayer(TL);
        PushLayer(new FullscreenBlitLayer(TL.Target));
        PushLayer(new DebugDrawLayer(TL.Target));
        
        PushOverlay(new ImGuiLayer());
    }
    protected override void OnUpdate(float deltaTime) {
        base.OnUpdate(deltaTime);

        if (Input.IsKeyDown(Keys.Escape)) {
            this.Close();
        }
        
        if (Input.IsKeyPressed(Keys.F1)) {
            LayerStack.First().IsVisible = !LayerStack.First().IsVisible;
        }
        
        _controller.Update(deltaTime);
        
        GL.LineWidth(8.0f);
        DebugDraw.Line(new Vector3(1,0,0), new Vector3(2,0,0), new Vector4(1, 0, 0, 1));
        
        DebugDraw.AABB(new Vector3(4,0,0), Vector3.One * 5, new Vector4(0, 1, 0, 1));

        DebugDraw.Transform(new Vector3(3, 0, 0), Quaternion.Identity, 0.5f);
        
        DebugDraw.Point(new Vector3(1,0,1), 0.5f, new Vector4(0,0, 1, 1));
        
        DebugDraw.Capsule(new Vector3(1,0,3), new Vector3(1,1,3), 0.5f, new Vector4(1,0,1,1));

        DebugDraw.Arrow(new Vector3(2, 0, 2), new Vector3(3, 1, -1), new Vector4(1, 1, 0, 1));
    }
    
    protected override void OnRender() {
        ImGui.Begin("DEBUG TEST");
        ImGui.Image(TL.Target.GetColorTexture(), new System.Numerics.Vector2(512, 512 / ((float)Application.Instance.Width / Application.Instance.Height)), new System.Numerics.Vector2(0,1),new System.Numerics.Vector2(1,0));
        ImGui.End();
        base.OnRender();
    }
    
}