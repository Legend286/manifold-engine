using Manifold.Core.InputSystem;
using Manifold.Runtime;
using Manifold.Sandbox.Layers;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sandbox;

public class SandboxApp : Application {
    
    public SandboxApp(string title) : base(title) {
        PushLayer(new TestLayer());

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
    }
    
}