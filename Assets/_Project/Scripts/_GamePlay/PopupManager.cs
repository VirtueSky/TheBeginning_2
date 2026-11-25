using System;
using System.Collections.Generic;
using Base.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VirtueSky.Inspector;
using Cysharp.Threading.Tasks;
using VirtueSky.Misc;
using VirtueSky.Pattern;
using VirtueSky.Utils;

namespace Base.Game
{
    [EditorIcon("icon_generator")]
    public class PopupManager : Singleton<PopupManager>
    {
        [HeaderLine("Environment", false, CustomColor.Aqua, CustomColor.Beige)] [SerializeField]
        private Transform parentContainer;

        private readonly Dictionary<Type, UIPopup> _container = new Dictionary<Type, UIPopup>();

        // Popups currently being loaded -> shared task so concurrent calls don't create duplicates
        private readonly Dictionary<Type, UniTask<UIPopup>> _loading = new Dictionary<Type, UniTask<UIPopup>>();

        // Addressables handles kept alive while popup instances exist, released on destroy
        private readonly Dictionary<Type, AsyncOperationHandle<GameObject>> _handles = new Dictionary<Type, AsyncOperationHandle<GameObject>>();

        private int index = 1;

        private async UniTaskVoid InternalShow<T>(bool isHideAll = true, Action showPopupCompleted = null, Action<UIPopup> setup = null)
        {
            UIPopup popup;
            try
            {
                popup = await GetOrCreate(typeof(T));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            if (popup == null || popup.isActiveAndEnabled) return;

            // Init data before Show() so OnBeforeShow can use it
            try
            {
                setup?.Invoke(popup);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (isHideAll)
            {
                InternalHideAll();
            }

            popup.Show();
            showPopupCompleted?.Invoke();
        }

        private async UniTask<UIPopup> GetOrCreate(Type type)
        {
            if (_container.TryGetValue(type, out UIPopup cached))
            {
                if (cached != null) return cached;
                // Instance was destroyed externally (e.g. scene unload) -> drop and recreate
                _container.Remove(type);
            }

            if (!_loading.TryGetValue(type, out UniTask<UIPopup> task))
            {
                task = LoadAndCreate(type).Preserve();
                _loading[type] = task;
            }

            try
            {
                return await task;
            }
            finally
            {
                _loading.Remove(type);
            }
        }

        private async UniTask<UIPopup> LoadAndCreate(Type type)
        {
            string key = GetKeyPopup(type.ToString());

            if (!_handles.TryGetValue(type, out AsyncOperationHandle<GameObject> handle) || !handle.IsValid())
            {
                handle = Addressables.LoadAssetAsync<GameObject>(key);
                _handles[type] = handle;
            }

            GameObject prefab;
            try
            {
                prefab = await handle;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load popup '{key}': {e.Message}".SetColor(Color.red));
                ReleaseHandle(type);
                return null;
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load popup '{key}': {handle.OperationException?.Message}".SetColor(Color.red));
                ReleaseHandle(type);
                return null;
            }

            UIPopup popupPrefab = prefab != null ? prefab.GetComponent<UIPopup>() : null;
            if (popupPrefab == null)
            {
                Debug.LogError($"Popup '{key}' has no UIPopup component".SetColor(Color.red));
                ReleaseHandle(type);
                return null;
            }

            if (this == null) return null; // manager destroyed while loading

            UIPopup popupInstance = Instantiate(popupPrefab, parentContainer);
            // Keep hidden until Show() is called
            popupInstance.gameObject.SetActive(false);
            popupInstance.canvas.sortingOrder = index++;
            _container[type] = popupInstance;
            return popupInstance;
        }

        private void ReleaseHandle(Type type)
        {
            if (_handles.TryGetValue(type, out AsyncOperationHandle<GameObject> handle))
            {
                if (handle.IsValid()) Addressables.Release(handle);
                _handles.Remove(type);
            }
        }

        private void InternalHide<T>(Action hidePopupCompleted = null)
        {
            if (_container.TryGetValue(typeof(T), out UIPopup popup) && popup != null)
            {
                if (popup.isActiveAndEnabled)
                {
                    popup.Hide();
                    hidePopupCompleted?.Invoke();
                }
            }
            else
            {
                Debug.Log("Popup not found to hide".SetColor(Color.red));
            }
        }

        private UIPopup InternalGet<T>()
        {
            return _container.TryGetValue(typeof(T), out UIPopup popup) && popup != null ? popup : null;
        }

        private bool InternalIsPopupReady<T>()
        {
            return InternalGet<T>() != null;
        }

        private void InternalHideAll()
        {
            foreach (var popup in _container.Values)
            {
                if (popup != null && popup.isActiveAndEnabled)
                {
                    popup.Hide();
                }
            }
        }

        private string GetKeyPopup(string fullName)
        {
            int index = fullName.LastIndexOf('.');
            if (index != -1)
            {
                return fullName.Substring(index + 1).Trim();
            }
            else
            {
                return fullName;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var popup in _container.Values)
            {
                if (popup != null) Destroy(popup.gameObject);
            }

            _container.Clear();

            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid()) Addressables.Release(handle);
            }

            _handles.Clear();
        }

        #region API

        public static void Show<T>(bool isHideAll = true, Action showPopupCompleted = null) =>
            Instance.InternalShow<T>(isHideAll, showPopupCompleted).Forget();

        /// <summary>
        /// Show popup and call <paramref name="setup"/> right before Show() (works even on first load).
        /// Example: PopupManager.Show&lt;PopupReward&gt;(p =&gt; p.Init(data));
        /// </summary>
        public static void Show<T>(Action<T> setup, bool isHideAll = true, Action showPopupCompleted = null) where T : UIPopup =>
            Instance.InternalShow<T>(isHideAll, showPopupCompleted, setup == null ? null : p => setup((T)p)).Forget();

        /// <summary>
        /// Load + instantiate popup (kept hidden) without showing it. Use to Init early or to avoid a hitch on first open.
        /// Example: var p = await PopupManager.Preload&lt;PopupShop&gt;(); p.Init(...);
        /// </summary>
        public static async UniTask<T> Preload<T>() where T : UIPopup => await Instance.GetOrCreate(typeof(T)) as T;

        public static void Hide<T>(Action hidePopupCompleted = null) =>
            Instance.InternalHide<T>(hidePopupCompleted);

        public static UIPopup Get<T>() => Instance.InternalGet<T>();
        public static bool IsPopupReady<T>() => Instance.InternalIsPopupReady<T>();
        public static void HideAll() => Instance.InternalHideAll();

        #endregion
    }
}
