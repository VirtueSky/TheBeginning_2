using System;
using System.Collections.Generic;
using UnityEngine;
using VirtueSky.Inspector;
using VirtueSky.Misc;
using VirtueSky.Utils;

namespace Base.UI
{
    [EditorIcon("icon_generator")]
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField] private PopupSettings popupSettings;

        [HeaderLine("Environment", false, CustomColor.Aqua, CustomColor.Beige)] [SerializeField]
        private Transform parentContainer;

        private readonly Dictionary<Type, UIPopup> _container = new Dictionary<Type, UIPopup>();

        private int index = 1;

        private void InternalShow<T>(bool isHideAll = true, Action showPopupCompleted = null)
        {
            _container.TryGetValue(typeof(T), out UIPopup popup);
            if (popup == null)
            {
                var popupPrefab = popupSettings.GetPrefabPopup(typeof(T).Name);
                if (popupPrefab != null)
                {
                    var popupInstance = Instantiate(popupPrefab, parentContainer);
                    if (isHideAll)
                    {
                        InternalHideAll();
                    }

                    popupInstance.Show();
                    showPopupCompleted?.Invoke();
                    _container.Add(popupInstance.GetType(), popupInstance);
                    popupInstance.canvas.sortingOrder = index++;
                }
                else
                {
                    Debug.Log("Popup not found in the list to show".SetColor(Color.red));
                }
            }
            else
            {
                if (!popup.isActiveAndEnabled)
                {
                    if (isHideAll)
                    {
                        InternalHideAll();
                    }

                    popup.Show();
                    showPopupCompleted?.Invoke();
                }
            }
        }

        private void InternalHide<T>(Action hidePopupCompleted = null)
        {
            if (_container.TryGetValue(typeof(T), out UIPopup popup))
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
            return _container.GetValueOrDefault(typeof(T));
        }

        private bool InternalIsPopupReady<T>()
        {
            return _container.ContainsKey(typeof(T));
        }

        private void InternalHideAll()
        {
            foreach (var popup in _container.Values)
            {
                if (popup.isActiveAndEnabled)
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

        #region API

        public static void Show<T>(bool isHideAll = true, Action showPopupCompleted = null) =>
            Instance.InternalShow<T>(isHideAll, showPopupCompleted);

        public static void Hide<T>(Action hidePopupCompleted = null) =>
            Instance.InternalHide<T>(hidePopupCompleted);

        public static UIPopup Get<T>() => Instance.InternalGet<T>();
        public static bool IsPopupReady<T>() => Instance.InternalIsPopupReady<T>();
        public static void HideAll() => Instance.InternalHideAll();

        #endregion
    }
}