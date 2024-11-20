using UnityEngine;

namespace Virtuesky.Events
{
    public abstract class EventsListenerMono : MonoBehaviour
    {
        public enum BindingListener
        {
            UNTIL_DISABLE,
            UNTIL_DESTROY
        }

        [SerializeField] private BindingListener bindingListener;

        protected abstract void ToggleListenerEvent(bool isListenerEvent);

        private void Awake()
        {
            if (bindingListener == BindingListener.UNTIL_DESTROY)
            {
                ToggleListenerEvent(true);
            }
        }

        private void OnEnable()
        {
            if (bindingListener == BindingListener.UNTIL_DISABLE)
            {
                ToggleListenerEvent(true);
            }
        }

        private void OnDisable()
        {
            if (bindingListener == BindingListener.UNTIL_DISABLE)
            {
                ToggleListenerEvent(false);
            }
        }

        private void OnDestroy()
        {
            if (bindingListener == BindingListener.UNTIL_DESTROY)
            {
                ToggleListenerEvent(false);
            }
        }
#if UNITY_EDITOR
        // public bool IsPlayingEditor() => UnityEditor.EditorApplication.isPlaying;
#endif
    }
}