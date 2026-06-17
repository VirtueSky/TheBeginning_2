using UnityEngine;
using VirtueSky.Core;

namespace VirtueSky.Pattern
{
    public abstract class Singleton<T> : BaseMono where T : MonoBehaviour
    {
        [SerializeField] private bool isDontDestroyOnLoad;
        static T _instance;

        public static T Instance => _instance;

        protected virtual void Awake()
        {
            if (isDontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }

            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Debug.LogError($"An instance of {typeof(T).Name} already exists. Destroying the new one.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }
    }
}