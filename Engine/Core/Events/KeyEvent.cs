using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Manifold.Core.Events;

public abstract class KeyEvent : Event {
    public Keys KeyCode { get; }

    protected KeyEvent(Keys keycode) {
        KeyCode = keycode;
    }

    public override EventCategory GetCategoryFlags() => EventCategory.Keyboard;
}

public class KeyPressedEvent : KeyEvent {
    public int RepeatCount { get; }

    public KeyPressedEvent(Keys keycode, int repeatCount) : base(keycode) {
        RepeatCount = repeatCount;
    }
    
    public override EventType GetEventType() => EventType.KeyPressed;
    public override string GetName() => $"KeyPressedEvent: {KeyCode} ({RepeatCount})";
    public override EventCategory GetCategoryFlags() => EventCategory.Keyboard;
}