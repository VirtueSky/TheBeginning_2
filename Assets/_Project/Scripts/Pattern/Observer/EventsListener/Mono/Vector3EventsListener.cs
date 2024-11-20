using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtueSky.Inspector;

namespace Virtuesky.Events
{
    [EditorIcon("scriptable_event_listener"), HideMonoScript]
    public class Vector3EventsListener : EventsListenerMono, IEventsListener<Vector3>
    {
        [Serializable]
        public class EventResponseData
        {
            [SearchableEnum] public EventName eventName;
            [Space] public UnityEvent<Vector3> response;
#if UNITY_EDITOR
            [ShowIf(nameof(IsPlayingEditor)), SerializeField]
            private Vector3 valueDebug;

            [ShowIf(nameof(IsPlayingEditor)), Button("Raise")]
            void RaiseDebug()
            {
                eventName.Raise(valueDebug);
            }

            bool IsPlayingEditor() => UnityEditor.EditorApplication.isPlaying;
#endif
        }

        [Space, SerializeField] private EventResponseData[] eventResponseDatas;

        private readonly Dictionary<EventName, UnityEvent<Vector3>> _dictionary =
            new Dictionary<EventName, UnityEvent<Vector3>>();

        protected override void ToggleListenerEvent(bool isListenerEvent)
        {
            if (isListenerEvent)
            {
                foreach (var t in eventResponseDatas)
                {
                    t.eventName.AddListener(this);
                    _dictionary.TryAdd(t.eventName, t.response);
                }
            }
            else
            {
                foreach (var t in eventResponseDatas)
                {
                    t.eventName.RemoveListener(this);
                    if (_dictionary.ContainsKey(t.eventName)) _dictionary.Remove(t.eventName);
                }
            }
        }

        public void OnEventRaised(EventName eventName, Vector3 value)
        {
            _dictionary[eventName]?.Invoke(value);
        }
    }
}