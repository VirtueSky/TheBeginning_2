using System;
using System.Collections.Generic;

public static partial class Events
{
    private static readonly Dictionary<EventName, Action<int>> _dictIntEvents = new Dictionary<EventName, Action<int>>();

    public static void AddListener(this EventName eventName, Action<int> @event)
    {
        if (!_dictIntEvents.TryAdd(eventName, @event))
            _dictIntEvents[eventName] += @event;
    }

    public static void RemoveListener(this EventName eventName, Action<int> @event)
    {
        if (_dictIntEvents.ContainsKey(eventName))
        {
            _dictIntEvents[eventName] -= @event;
            if (_dictIntEvents[eventName] == null) _dictIntEvents.Remove(eventName);
        }
    }

    public static void Raise(this EventName eventName, int value)
    {
        if (_dictIntEvents.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
    }
}