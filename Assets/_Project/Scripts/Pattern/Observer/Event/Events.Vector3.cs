using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class Events
{
    private static readonly Dictionary<EventName, Action<Vector3>> _dictVector3Events = new Dictionary<EventName, Action<Vector3>>();

    public static void AddListener(this EventName eventName, Action<Vector3> @event)
    {
        if (!_dictVector3Events.TryAdd(eventName, @event))
            _dictVector3Events[eventName] += @event;
    }

    public static void RemoveListener(this EventName eventName, Action<Vector3> @event)
    {
        if (_dictVector3Events.ContainsKey(eventName))
        {
            _dictVector3Events[eventName] -= @event;
            if (_dictVector3Events[eventName] == null) _dictVector3Events.Remove(eventName);
        }
    }

    public static void Raise(this EventName eventName, Vector3 value)
    {
        if (_dictVector3Events.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
    }
}