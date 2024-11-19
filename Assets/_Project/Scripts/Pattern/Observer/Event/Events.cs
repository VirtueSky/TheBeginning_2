using System;
using System.Collections.Generic;

public static partial class Events
{
    private static readonly Dictionary<EventName, Action> _dictionaryEvents = new Dictionary<EventName, Action>();

    public static void AddListener(this EventName eventName, Action @event)
    {
        if (!_dictionaryEvents.TryAdd(eventName, @event))
            _dictionaryEvents[eventName] += @event;
    }

    public static void RemoveListener(this EventName eventName, Action @event)
    {
        if (_dictionaryEvents.ContainsKey(eventName))
        {
            _dictionaryEvents[eventName] -= @event;
            if (_dictionaryEvents[eventName] == null) _dictionaryEvents.Remove(eventName);
        }
    }

    public static void Raise(this EventName eventName)
    {
        if (_dictionaryEvents.TryGetValue(eventName, out var @event)) @event?.Invoke();
    }
}