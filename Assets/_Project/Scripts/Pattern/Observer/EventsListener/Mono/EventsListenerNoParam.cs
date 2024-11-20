using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtueSky.Inspector;

namespace Virtuesky.Events
{
    [EditorIcon("scriptable_event_listener"), HideMonoScript]
    public class EventsListenerNoParam : EventsListenerMono, IEventsListener
    {
        [Serializable]
        public class EventResponseData
        {
            [SearchableEnum] public EventName eventName;
            [Space] public UnityEvent response;
#if UNITY_EDITOR
            [ShowIf(nameof(IsPlayingEditor)), Button("Raise")]
            void RaiseDebug()
            {
                eventName.Raise();
            }

            bool IsPlayingEditor() => UnityEditor.EditorApplication.isPlaying;
#endif
        }

        [Space, SerializeField] private EventResponseData[] eventResponseDatas;

        private readonly Dictionary<EventName, UnityEvent> _dictionary =
            new Dictionary<EventName, UnityEvent>();

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

        public void OnEventRaised(EventName eventName)
        {
            _dictionary[eventName]?.Invoke();
        }
    }
}