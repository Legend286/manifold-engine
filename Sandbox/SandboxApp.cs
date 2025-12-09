using Manifold.Core.InputSystem;
using Manifold.Core.Renderer.Debug;
using Manifold.Core.Renderer.Debug.Layers;
using Manifold.Runtime;
using Manifold.Sandbox.Layers;
using OpenTK.Graphics.OpenGLES2;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sandbox;

public class SandboxApp : Application {
    
    public SandboxApp(string title) : base(title) {
        PushLayer(new TestLayer());
        PushLayer(new DebugDrawLayer());

        var imguiLayer = new ImGuiLayer();
        PushOverlay(imguiLayer);
    }
    protected override void OnUpdate(float deltaTime) {
        base.OnUpdate(deltaTime);

        if (Input.IsKeyDown(Keys.Escape)) {
            this.Close();
        }
        
        if (Input.IsKeyPressed(Keys.F1)) {
            LayerStack.Last().IsVisible = !LayerStack.Last().IsVisible;
        }
    }
    
    protected override void OnRender() {
        base.OnRender();

        GL.LineWidth(8.0f);
        DebugDraw.Line(new Vector3(1,0,0), new Vector3(2,0,0), new Vector4(1, 0, 0, 1));
        
        DebugDraw.AABB(new Vector3(4,0,0), Vector3.One * 5, new Vector4(0, 1, 0, 1));

        DebugDraw.Transform(new Vector3(3, 0, 0), Quaternion.Identity, 0.5f);
        
        DebugDraw.Point(new Vector3(1,0,1), 0.5f, new Vector4(0,0, 1, 1));
        
        DebugDraw.Capsule(new Vector3(1,0,3), new Vector3(1,1,3), 0.5f, new Vector4(1,0,1,1));

        DebugDraw.Arrow(new Vector3(2, 0, 2), new Vector3(3, 1, -1), new Vector4(1, 1, 0, 1));
    }
    
}