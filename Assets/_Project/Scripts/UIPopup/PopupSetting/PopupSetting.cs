using UnityEngine;
using UnityEngine.UI;

namespace Base.UI
{
    public class PopupSetting : UIPopup
    {
        [SerializeField] private Button buttonRestore;

        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            buttonRestore.gameObject.SetActive(false);
#if UNITY_IOS
            buttonRestore.gameObject.SetActive(true);
#endif
            buttonRestore.onClick.AddListener(OnClickRestorePurchase);
        }

        protected override void OnBeforeHide()
        {
            base.OnBeforeHide();
            buttonRestore.onClick.RemoveListener(OnClickRestorePurchase);
        }

        public void OnClickRestorePurchase()
        {
        }
    }
}