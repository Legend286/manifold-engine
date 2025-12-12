namespace Manifold.Core.Events;

public class WindowResizeEvent : Event {
    public int Width { get; }

    public int Height { get; }

    public WindowResizeEvent(int width, int height) {
        Width = width;
        Height = height;
    }


    public override EventType GetEventType() => EventType.WindowResize;
    public override string GetName() => $"WindowResizeEvent: {Width}, {Height}";
    public override EventCategory GetCategoryFlags() => EventCategory.Application;
}

public class WindowCloseEvent : Event {
    public override EventType GetEventType() => EventType.WindowClose;
    public override string GetName() => "WindowCloseEvent";
    public override EventCategory GetCategoryFlags() => EventCategory.Application;
}