using System;
using System.Collections.Generic;

namespace Virtuesky.Events
{
    public static partial class Events
    {
        private static readonly Dictionary<EventName, Action<float>> _dictFloatEvents =
            new Dictionary<EventName, Action<float>>();

        private static readonly List<IEventsListener<float>> listenersFloat =
            new List<IEventsListener<float>>();

        public static void AddListener(this EventName eventName, Action<float> @event)
        {
            if (!_dictFloatEvents.TryAdd(eventName, @event))
                _dictFloatEvents[eventName] += @event;
        }

        public static void RemoveListener(this EventName eventName, Action<float> @event)
        {
            if (_dictFloatEvents.ContainsKey(eventName))
            {
                _dictFloatEvents[eventName] -= @event;
                if (_dictFloatEvents[eventName] == null) _dictFloatEvents.Remove(eventName);
            }
        }

        public static void AddListener(this EventName eventName, IEventsListener<float> listener)
        {
            if (!listenersFloat.Contains(listener)) listenersFloat.Add(listener);
        }

        public static void RemoveListener(this EventName eventName, IEventsListener<float> listener)
        {
            if (listenersFloat.Contains(listener)) listenersFloat.Remove(listener);
        }

        public static void Raise(this EventName eventName, float value)
        {
            if (_dictFloatEvents.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
            for (int i = listenersFloat.Count - 1; i >= 0; i--)
            {
                listenersFloat[i].OnEventRaised(eventName, value);
            }
        }
    }
}