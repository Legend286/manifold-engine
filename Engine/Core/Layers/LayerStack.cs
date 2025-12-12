using System.Collections;

namespace Manifold.Core.Layers;

public class LayerStack : IEnumerable<Layer> {
    private readonly List<Layer> _layers = new List<Layer>();
    private int _layerInsertIndex = 0;

    public void PushLayer(Layer layer) {
        _layers.Insert(_layerInsertIndex, layer);
        _layerInsertIndex++;
        layer.OnAttach();
    }

    public void PushOverlay(Layer overlay) {
        _layers.Add(overlay);
        overlay.OnAttach();
    }

    public void PopLayer(Layer layer) {
        if (_layers.Remove(layer)) {
            _layerInsertIndex--;
            layer.OnDetach();
        }
    }

    public void PopOverlay(Layer overlay) {
        if (_layers.Remove(overlay)) {
            overlay.OnDetach();
        }
    }
    public IEnumerator<Layer> GetEnumerator() => _layers.GetEnumerator(); 
    
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}