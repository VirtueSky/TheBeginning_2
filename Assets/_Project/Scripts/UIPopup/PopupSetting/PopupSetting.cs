#if VIRTUESKY_ADMOB
using GoogleMobileAds.Ump.Api;
#endif
using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Ads;

namespace Base.UI
{
    public class PopupSetting : UIPopup
    {
        [SerializeField] private Button buttonRestore;
        [SerializeField] private Button buttonPrivacyConsent;
        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
           SetupButtonDefault();
#if UNITY_IOS
            buttonRestore.gameObject.SetActive(true);
#endif
#if VIRTUESKY_ADMOB
            buttonPrivacyConsent.gameObject.SetActive(ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required);
#endif
            buttonRestore.onClick.AddListener(OnClickRestorePurchase);
            buttonPrivacyConsent.onClick.AddListener(OnClickShowAgainGDPR);
        }
        protected override void OnBeforeHide()
        {
            base.OnBeforeHide();
            buttonRestore.onClick.RemoveListener(OnClickRestorePurchase);
            buttonPrivacyConsent.onClick.RemoveListener(OnClickShowAgainGDPR);
        }

        void SetupButtonDefault()
        {
            buttonRestore.gameObject.SetActive(false);
            buttonPrivacyConsent.gameObject.SetActive(false);
        }
         void OnClickRestorePurchase()
        {
        }

        void OnClickShowAgainGDPR()
        {
#if VIRTUESKY_ADMOB
            Advertising.ShowAgainGdpr();
#endif
        }
    }
}