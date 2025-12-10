using ImGuiNET;
using Manifold.Core.Events;
using Manifold.Core.Layers;
using Manifold.Core.UI;
using Manifold.Runtime;
using Sandbox;

namespace Manifold.Sandbox.Layers;

public class ImGuiLayer : Layer {
    private ImGuiRenderer _renderer;

    public ImGuiLayer() : base("ImGui Layer") {

    }

    public override void OnAttach() {
        base.OnAttach();
        _renderer = new ImGuiRenderer(Application.Instance.Width, Application.Instance.Height);
    }

    public override void OnRender() {
        _renderer.Render();
    }

    public override void OnUpdate(float deltaTime) {
        _renderer.Update(Application.Instance.OpenTKWindow, deltaTime);
        
        ImGui.Begin("WOW");
        bool visible = Application.Instance.LayerStack.First().IsVisible;

        if (ImGui.Checkbox("Show triangle layer", ref visible)) {
            Application.Instance.LayerStack.First().IsVisible = visible;
        }
        ImGui.End();
        
        
    }

    public override void OnEvent(Event e) {
        base.OnEvent(e);

        if (e.GetEventType() == EventType.WindowResize) {
            
            var resizeEvent = (WindowResizeEvent)e;

            _renderer.WindowResized(resizeEvent.Width, resizeEvent.Height);
        }

    }

}