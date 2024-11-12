#if VIRTUESKY_ADMOB
using GoogleMobileAds.Ump.Api;
#endif
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Ads;
using VirtueSky.Localization;
using VirtueSky.Misc;
using VirtueSky.Utils;

namespace Base.UI
{
    public class PopupSetting : UIPopup
    {
        [SerializeField] private Button buttonRestore;
        [SerializeField] private Button buttonPrivacyConsent;
        [SerializeField] private UIEffect effectButtonEn;
        [SerializeField] private UIEffect effectButtonVi;

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
            SetupUiButtonLanguage();
        }

        protected override void OnBeforeHide()
        {
            base.OnBeforeHide();
            buttonRestore.onClick.RemoveListener(OnClickRestorePurchase);
            buttonPrivacyConsent.onClick.RemoveListener(OnClickShowAgainGDPR);
        }

        void SetupUiButtonLanguage()
        {
            effectButtonEn.toneIntensity = 1;
            effectButtonVi.toneIntensity = 1;
            if (Locale.CurrentLanguage == Language.English) effectButtonEn.toneIntensity = 0;
            if (Locale.CurrentLanguage == Language.Vietnamese) effectButtonVi.toneIntensity = 0;
        }

        public void OnClickChangeLanguageEn()
        {
            Locale.CurrentLanguage = Language.English;
            SetupUiButtonLanguage();
        }

        public void OnClickChangeLanguageVi()
        {
            Locale.CurrentLanguage = Language.Vietnamese;
            SetupUiButtonLanguage();
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