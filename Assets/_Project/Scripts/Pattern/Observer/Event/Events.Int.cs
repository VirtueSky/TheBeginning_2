using System;
using System.Collections.Generic;

namespace Virtuesky.Events
{
    public static partial class Events
    {
        private static readonly Dictionary<EventName, Action<int>> _dictIntEvents =
            new Dictionary<EventName, Action<int>>();

        private static readonly List<IEventsListener<int>> listenersInt = new List<IEventsListener<int>>();

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

        public static void AddListener(this EventName eventName, IEventsListener<int> listener)
        {
            if (!listenersInt.Contains(listener)) listenersInt.Add(listener);
        }

        public static void RemoveListener(this EventName eventName, IEventsListener<int> listener)
        {
            if (listenersInt.Contains(listener)) listenersInt.Remove(listener);
        }

        public static void Raise(this EventName eventName, int value)
        {
            if (_dictIntEvents.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
            for (int i = listenersInt.Count - 1; i >= 0; i--)
            {
                listenersInt[i].OnEventRaised(eventName, value);
            }
        }
    }
}