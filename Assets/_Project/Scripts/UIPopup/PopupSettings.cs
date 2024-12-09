using System.Collections.Generic;
using UnityEngine;
using VirtueSky.Inspector;
using VirtueSky.Linq;

namespace Base.UI
{
    [EditorIcon("icon_scriptable"), HideMonoScript]
    public class PopupSettings : ScriptableObject
    {
        [SerializeField] private string pathLoad = "Assets/_Project/Prefabs/Popups";
        [SerializeField] private List<UIPopup> listUiPopups;

        public List<UIPopup> ItemPopupConfigs => listUiPopups;

        public UIPopup GetPrefabPopup(string popupName)
        {
            return listUiPopups.FirstOrDefault(item => item.name == popupName);
        }
#if UNITY_EDITOR
        [Button]
        void LoadPrefabPopup()
        {
            listUiPopups.Clear();
            var popups = VirtueSky.UtilsEditor.FileExtension.GetPrefabsFromFolder<UIPopup>(pathLoad);
            foreach (var obj in popups)
            {
                if (obj != null)
                {
                    listUiPopups.Add(obj);
                }
            }
        }
#endif
    }
}