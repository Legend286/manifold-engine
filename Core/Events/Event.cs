namespace Manifold.Core.Events;

[Flags]
public enum EventCategory {
    None = 0,
    Application = 1 << 0,
    Input = 1 << 1,
    Keyboard = 1 << 2,
    Mouse = 1 << 3,
    MouseButton = 1 << 4,
}

public enum EventType {
    None = 0,
    WindowClose, WindowResize, WindowFocus, WindowLostFocus,
    AppTick, AppUpdate, AppRender,
    KeyPressed, KeyReleased, KeyTyped,
    MouseButtonPressed, MouseButtonReleased, MouseMoved, MouseScrolled
}

public abstract class Event {
    public bool Handled { get; set; } = false;

    public abstract EventType GetEventType();
    public abstract string GetName();
    public abstract EventCategory GetCategoryFlags();

    public bool IsInCategory(EventCategory category) {
        return (GetCategoryFlags() & category) != 0;
    }

    public override string ToString() => GetName();
}