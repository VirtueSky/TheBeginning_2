using System;
using System.Collections.Generic;

namespace Virtuesky.Events
{
    public static partial class Events
    {
        private static readonly Dictionary<EventName, Action<string>> _dictStringEvents =
            new Dictionary<EventName, Action<string>>();

        private static readonly List<IEventsListener<string>> listenersString =
            new List<IEventsListener<string>>();

        public static void AddListener(this EventName eventName, Action<string> @event)
        {
            if (!_dictStringEvents.TryAdd(eventName, @event))
                _dictStringEvents[eventName] += @event;
        }

        public static void RemoveListener(this EventName eventName, Action<string> @event)
        {
            if (_dictStringEvents.ContainsKey(eventName))
            {
                _dictStringEvents[eventName] -= @event;
                if (_dictStringEvents[eventName] == null) _dictStringEvents.Remove(eventName);
            }
        }

        public static void AddListener(this EventName eventName, IEventsListener<string> listener)
        {
            if (!listenersString.Contains(listener)) listenersString.Add(listener);
        }

        public static void RemoveListener(this EventName eventName, IEventsListener<string> listener)
        {
            if (listenersString.Contains(listener)) listenersString.Remove(listener);
        }

        public static void Raise(this EventName eventName, string value)
        {
            if (_dictStringEvents.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
            for (int i = listenersString.Count - 1; i >= 0; i--)
            {
                listenersString[i].OnEventRaised(eventName, value);
            }
        }
    }
}