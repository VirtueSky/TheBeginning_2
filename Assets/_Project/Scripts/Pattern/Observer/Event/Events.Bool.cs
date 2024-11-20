using System;
using System.Collections.Generic;

namespace Virtuesky.Events
{
    public static partial class Events
    {
        private static readonly Dictionary<EventName, Action<bool>> _dictBoolEvents =
            new Dictionary<EventName, Action<bool>>();

        private static readonly List<IEventsListener<bool>> listenersBool = new List<IEventsListener<bool>>();

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

        public static void AddListener(this EventName eventName, IEventsListener<bool> listener)
        {
            if (!listenersBool.Contains(listener)) listenersBool.Add(listener);
        }

        public static void RemoveListener(this EventName eventName, IEventsListener<bool> listener)
        {
            if (listenersBool.Contains(listener)) listenersBool.Remove(listener);
        }

        public static void Raise(this EventName eventName, bool value)
        {
            if (_dictBoolEvents.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
            for (int i = listenersBool.Count - 1; i >= 0; i--)
            {
                listenersBool[i].OnEventRaised(eventName, value);
            }
        }
    }
}