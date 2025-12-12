namespace Manifold.Core.Events;

public class EventDispatcher {
    private readonly Event _event;

    public EventDispatcher(Event eventToDispatch) {
        _event = eventToDispatch;
    }

    public bool Dispatch<T>(Func<T, bool> func) where T : Event {
        if (_event is T candidate) {
            if (_event.Handled)
                return false;
            _event.Handled = func(candidate);

            return true;
        }
        
        return false;
    }
}