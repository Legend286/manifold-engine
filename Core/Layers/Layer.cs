using Manifold.Core.Events;

namespace Manifold.Core.Layers;

public abstract class Layer {
    public string Name { get; }

    public bool IsVisible = true;

    protected Layer(string name = "Layer") {
        Name = name;
    }

    public virtual void OnAttach() {
    }

    public virtual void OnDetach() {
    }

    public virtual void OnRender() {
    }
    
    public virtual void OnUpdate(float deltaTime) {
    }
    
    public virtual void OnEvent(Event e) {
    }
}