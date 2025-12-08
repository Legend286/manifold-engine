using Manifold.Core.Events;

namespace Manifold.Core.Windowing;

public interface IWindow {
    int Width { get; }
    int Height { get; }
    string Title { get; }
    bool IsVSyncEnabled { get; }

    void SetVSync(bool enabled);
    void OnUpdate();
    void Close();

    void SetEventCallback(Action<Event> callback);
}