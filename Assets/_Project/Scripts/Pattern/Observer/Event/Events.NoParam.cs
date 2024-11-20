using System;
using System.Collections.Generic;

namespace Virtuesky.Events
{
    public static partial class Events
    {
        private static readonly Dictionary<EventName, Action> _dictionaryEvents =
            new Dictionary<EventName, Action>();

        private static readonly List<IEventsListener> listeners = new List<IEventsListener>();

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

        public static void AddListener(this EventName eventName, IEventsListener listener)
        {
            if (!listeners.Contains(listener)) listeners.Add(listener);
        }

        public static void RemoveListener(this EventName eventName, IEventsListener listener)
        {
            if (listeners.Contains(listener)) listeners.Remove(listener);
        }

        public static void Raise(this EventName eventName)
        {
            if (_dictionaryEvents.TryGetValue(eventName, out var @event)) @event?.Invoke();
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(eventName);
            }
        }
    }
}