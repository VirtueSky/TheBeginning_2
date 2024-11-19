using System;
using System.Collections.Generic;

public static partial class Events
{
    private static readonly Dictionary<EventName, Action<bool>> _dictBoolEvents = new Dictionary<EventName, Action<bool>>();

    public static void AddListener(this EventName eventName, Action<bool> @event)
    {
        if (!_dictBoolEvents.TryAdd(eventName, @event))
            _dictBoolEvents[eventName] += @event;
    }

    public static void RemoveListener(this EventName eventName, Action<bool> @event)
    {
        if (_dictBoolEvents.ContainsKey(eventName))
        {
            _dictBoolEvents[eventName] -= @event;
            if (_dictBoolEvents[eventName] == null) _dictBoolEvents.Remove(eventName);
        }
    }

    public static void Raise(this EventName eventName, bool value)
    {
        if (_dictBoolEvents.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
    }
}