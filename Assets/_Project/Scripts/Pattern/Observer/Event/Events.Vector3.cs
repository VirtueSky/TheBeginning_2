using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuesky.Events
{
    public static partial class Events
    {
        private static readonly Dictionary<EventName, Action<Vector3>> _dictVector3Events =
            new Dictionary<EventName, Action<Vector3>>();

        private static readonly List<IEventsListener<Vector3>> listenersV3 =
            new List<IEventsListener<Vector3>>();

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

        public static void AddListener(this EventName eventName, IEventsListener<Vector3> listener)
        {
            if (!listenersV3.Contains(listener)) listenersV3.Add(listener);
        }

        public static void RemoveListener(this EventName eventName, IEventsListener<Vector3> listener)
        {
            if (listenersV3.Contains(listener)) listenersV3.Remove(listener);
        }

        public static void Raise(this EventName eventName, Vector3 value)
        {
            if (_dictVector3Events.TryGetValue(eventName, out var @event)) @event?.Invoke(value);
            for (int i = listenersV3.Count - 1; i >= 0; i--)
            {
                listenersV3[i].OnEventRaised(eventName, value);
            }
        }
    }
}