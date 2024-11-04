using System;
using System.Collections.Generic;
using Base.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using VirtueSky.Core;
using VirtueSky.Inspector;
using Cysharp.Threading.Tasks;
using VirtueSky.Misc;

namespace Base.Game
{
    [EditorIcon("icon_generator")]
    public class PopupManager : Singleton<PopupManager>
    {
        [HeaderLine("Environment")] [SerializeField]
        private Transform parentContainer;

        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Camera cameraUI;

        private readonly Dictionary<Type, UIPopup> _container = new Dictionary<Type, UIPopup>();

        private int index = 1;

        private async void InternalShow<T>(bool isHideAll = true)
        {
            _container.TryGetValue(typeof(T), out UIPopup popup);
            if (popup == null)
            {
                var obj = await Addressables.LoadAssetAsync<GameObject>(GetKeyPopup(typeof(T).ToString()));
                var popupPrefab = obj.GetComponent<UIPopup>();
                if (popupPrefab != null)
                {
                    var popupInstance = Instantiate(popupPrefab, parentContainer);
                    if (isHideAll)
                    {
                        InternalHideAll();
                    }

                    popupInstance.Show();
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
                }
            }
        }

        private void InternalHide<T>()
        {
            if (_container.TryGetValue(typeof(T), out UIPopup popup))
            {
                if (popup.isActiveAndEnabled)
                {
                    popup.Hide();
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

        public static void Show<T>(bool isHideAll = true) => Instance.InternalShow<T>(isHideAll);
        public static void Hide<T>() => Instance.InternalHide<T>();
        public static UIPopup Get<T>() => Instance.InternalGet<T>();
        public static bool IsPopupReady<T>() => Instance.InternalIsPopupReady<T>();
        public static void HideAll() => Instance.InternalHideAll();

        #endregion
    }
}