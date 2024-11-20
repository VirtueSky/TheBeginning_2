using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VirtueSky.Inspector;

namespace Virtuesky.Events
{
    [EditorIcon("scriptable_event_listener"), HideMonoScript]
    public class BoolEventsListener : EventsListenerMono, IEventsListener<bool>
    {
        [Serializable]
        public class EventResponseData
        {
            [SearchableEnum] public EventName eventName;
            [Space] public UnityEvent<bool> response;
#if UNITY_EDITOR
            [ShowIf(nameof(IsPlayingEditor)), SerializeField]
            private bool valueDebug;

            [ShowIf(nameof(IsPlayingEditor)), Button("Raise")]
            void RaiseDebug()
            {
                eventName.Raise(valueDebug);
            }

            bool IsPlayingEditor() => UnityEditor.EditorApplication.isPlaying;
#endif
        }

        [Space, SerializeField] private EventResponseData[] eventResponseDatas;

        private readonly Dictionary<EventName, UnityEvent<bool>> _dictionary =
            new Dictionary<EventName, UnityEvent<bool>>();

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

        public void OnEventRaised(EventName eventName, bool value)
        {
            _dictionary[eventName]?.Invoke(value);
        }
    }
}