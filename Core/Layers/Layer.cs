using Manifold.Core.Events;

namespace Manifold.Core.Layers;

public abstract class Layer {
    public string Name { get; }

    private bool _isVisible = true;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;

            if (_isVisible) {
                OnEnable();
            }
            else {
                OnDisable();
            }
        }
    }

    protected Layer(string name = "Layer") {
        Name = name;
    }
    public virtual void OnAttach() {
    }
    public virtual void OnDetach() {
    }
    public virtual void OnEnable() {
        
    }
    public virtual void OnDisable() {
        
    }
    public virtual void OnRender() {
    }
    public virtual void OnUpdate(float deltaTime) {
    }
    public virtual void OnEvent(Event e) {
    }
}