using Base.Data;
using Base.Game;
using Base.Global;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Ads;
using VirtueSky.Inspector;

namespace Base.Services
{
    public class AdminController : MonoBehaviour
    {
        [HeaderLine("UI")] [ReadOnly, SerializeField]
        private bool isShow = false;

        [SerializeField] private RectTransform container;
        [SerializeField] private GameObject holder;
        [SerializeField] private Button btnShowHideAdmin;
        [SerializeField] private Image iconButtonShowAdmin;
        [SerializeField] private Sprite iconBtnShow;
        [SerializeField] private Sprite iconBtnHide;
        [SerializeField] private Toggle toggleOffUI;
        [SerializeField] private Toggle toggleIsTesting;
        [SerializeField] private Toggle toggleOffInterAds;
        [SerializeField] private Toggle toggleOffBannerAds;
        [SerializeField] private Toggle toggleOffRewardAds;
        [SerializeField] private TMP_InputField inputFieldCurrency;
        [SerializeField] private TMP_InputField inputFieldLevel;
        [SerializeField] private Button btnNextLevel;
        [SerializeField] private Button btnPrevLevel;
        [SerializeField] private Button btnWinLevel;
        [SerializeField] private Button btnLoseLevel;
        [SerializeField] private Button btnJumpToLevel;
        [SerializeField] private Button btnEnterCurrency;
        [SerializeField] private Button btnAddCurrency;
        [SerializeField] private Button btnUnlockAllSkin;
        [SerializeField] private Button btnShowBanner;
        [SerializeField] private Button btnHideBanner;
        [SerializeField] private Button btnShowInter;
        [SerializeField] private Button btnShowReward;
        [SerializeField] private Button btnModifyConsent;
        [SerializeField] private ItemConfig itemConfig;
        [SerializeField] private GameConfig gameConfig;
        private bool isRequireGDPR = false;

        private void Awake()
        {
            gameObject.SetActive(gameConfig.enableAdministrator);
            AdStatic.OnPrivacyRequiredGDPR += OnChangeRequireGDPR;
            Observer.OnChangeStateGame += OnChangeStateGame;
        }

        private void OnDestroy()
        {
            AdStatic.OnPrivacyRequiredGDPR -= OnChangeRequireGDPR;
            Observer.OnChangeStateGame -= OnChangeStateGame;
        }

        void OnChangeRequireGDPR(bool isRequire)
        {
            isRequireGDPR = isRequire;
        }

        void OnChangeStateGame(GameState gameState)
        {
            Refresh();
        }

        private void OnEnable()
        {
            SetupDefault();
            btnModifyConsent.gameObject.SetActive(isRequireGDPR);
            // button
            btnModifyConsent.onClick.AddListener(OnClickModifyConsent);
            btnJumpToLevel.onClick.AddListener(OnClickJumpToLevel);
            btnEnterCurrency.onClick.AddListener(OnClickEnterCurrency);
            btnAddCurrency.onClick.AddListener(OnClickAdd10000Coin);
            btnUnlockAllSkin.onClick.AddListener(OnClickUnlockAllSkins);
            btnShowBanner.onClick.AddListener(OnClickShowBanner);
            btnHideBanner.onClick.AddListener(OnClickHideBanner);
            btnShowInter.onClick.AddListener(OnClickShowInter);
            btnShowReward.onClick.AddListener(OnClickShowReward);
            btnShowHideAdmin.onClick.AddListener(OnClickShowHideAdmin);
            btnNextLevel.onClick.AddListener(OnClickNextLevel);
            btnPrevLevel.onClick.AddListener(OnClickPreviousLevel);
            btnWinLevel.onClick.AddListener(OnClickWinLevel);
            btnLoseLevel.onClick.AddListener(OnClickLoseLevel);
            // toggle
            toggleOffUI.onValueChanged.AddListener(OnChangeOffUI);
            toggleIsTesting.onValueChanged.AddListener(OnChangeOffIsTesting);
            toggleOffBannerAds.onValueChanged.AddListener(OnChangeOffBanner);
            toggleOffInterAds.onValueChanged.AddListener(OnChangeOffInter);
            toggleOffRewardAds.onValueChanged.AddListener(OnChangeOffReward);
        }

        private void OnDisable()
        {
            // button
            btnModifyConsent.onClick.RemoveListener(OnClickModifyConsent);
            btnJumpToLevel.onClick.RemoveListener(OnClickJumpToLevel);
            btnEnterCurrency.onClick.RemoveListener(OnClickEnterCurrency);
            btnAddCurrency.onClick.RemoveListener(OnClickAdd10000Coin);
            btnUnlockAllSkin.onClick.RemoveListener(OnClickUnlockAllSkins);
            btnShowBanner.onClick.RemoveListener(OnClickShowBanner);
            btnHideBanner.onClick.RemoveListener(OnClickHideBanner);
            btnShowInter.onClick.RemoveListener(OnClickShowInter);
            btnShowReward.onClick.RemoveListener(OnClickShowReward);
            btnShowHideAdmin.onClick.RemoveListener(OnClickShowHideAdmin);
            btnNextLevel.onClick.RemoveListener(OnClickNextLevel);
            btnPrevLevel.onClick.RemoveListener(OnClickPreviousLevel);
            btnWinLevel.onClick.RemoveListener(OnClickWinLevel);
            btnLoseLevel.onClick.RemoveListener(OnClickLoseLevel);
            // toggle
            toggleOffUI.onValueChanged.RemoveListener(OnChangeOffUI);
            toggleIsTesting.onValueChanged.RemoveListener(OnChangeOffIsTesting);
            toggleOffBannerAds.onValueChanged.RemoveListener(OnChangeOffBanner);
            toggleOffInterAds.onValueChanged.RemoveListener(OnChangeOffInter);
            toggleOffRewardAds.onValueChanged.RemoveListener(OnChangeOffReward);
        }

        void SetupDefault()
        {
            Init();
            Refresh();
        }


        void Refresh()
        {
            toggleOffUI.isOn = UserData.IsOffUIAdministrator;
            toggleIsTesting.isOn = UserData.IsTestingAdministrator;
            toggleOffBannerAds.isOn = UserData.IsTestOffBannerAdsAdministrator;
            toggleOffInterAds.isOn = UserData.IsOffInterAdsAdministrator;
            toggleOffRewardAds.isOn = UserData.IsOffRewardAdsAdministrator;

            btnShowBanner.gameObject.SetActive(Application.isMobilePlatform);
            btnHideBanner.gameObject.SetActive(Application.isMobilePlatform);
            btnShowInter.gameObject.SetActive(Application.isMobilePlatform);
            btnShowReward.gameObject.SetActive(Application.isMobilePlatform);

            btnNextLevel.gameObject.SetActive(GameManager.Instance != null &&
                                              GameManager.Instance.GameState == GameState.PlayingLevel);
            btnPrevLevel.gameObject.SetActive(GameManager.Instance != null &&
                                              GameManager.Instance.GameState == GameState.PlayingLevel);
            btnWinLevel.gameObject.SetActive(GameManager.Instance != null &&
                                             GameManager.Instance.GameState == GameState.PlayingLevel);
            btnLoseLevel.gameObject.SetActive(GameManager.Instance != null &&
                                              GameManager.Instance.GameState == GameState.PlayingLevel);
        }

        void OnClickJumpToLevel()
        {
            if (inputFieldLevel.text != "")
            {
                UserData.CurrentLevel = int.Parse(inputFieldLevel.text);
            }

            inputFieldLevel.text = "";
            GameManager.Instance.PlayCurrentLevel();
        }

        void OnClickEnterCurrency()
        {
            if (inputFieldCurrency.text != "")
            {
                UserData.CoinTotal = int.Parse(inputFieldCurrency.text);
            }

            inputFieldCurrency.text = "";
        }

        void OnClickAdd10000Coin()
        {
            UserData.CoinTotal += 10000;
        }

        void OnClickShowBanner()
        {
            Advertising.Instance.ShowBanner();
        }

        void OnClickHideBanner()
        {
            Advertising.Instance.HideBanner();
        }

        void OnClickShowInter()
        {
            Advertising.Instance.ShowInterstitial();
        }

        void OnClickShowReward()
        {
            Advertising.Instance.ShowReward();
        }

        void OnClickUnlockAllSkins()
        {
            itemConfig.UnlockAllSkins();
        }

        void OnClickNextLevel()
        {
            GameManager.Instance.NextLevel();
        }

        void OnClickPreviousLevel()
        {
            GameManager.Instance.BackLevel();
        }

        void OnClickWinLevel()
        {
            GameManager.Instance.WinLevel(1.5f);
        }

        void OnClickLoseLevel()
        {
            GameManager.Instance.LoseLevel(1.5f);
        }

        void OnChangeOffIsTesting(bool isOn)
        {
            UserData.IsTestingAdministrator = isOn;
        }

        void OnChangeOffInter(bool isOn)
        {
            UserData.IsOffInterAdsAdministrator = isOn;
        }

        void OnChangeOffReward(bool isOn)
        {
            UserData.IsOffRewardAdsAdministrator = isOn;
        }

        void OnChangeOffBanner(bool isOn)
        {
            UserData.IsTestOffBannerAdsAdministrator = isOn;
        }

        void OnChangeOffUI(bool isOn)
        {
            UserData.IsOffUIAdministrator = isOn;
        }

        void OnClickModifyConsent()
        {
#if VIRTUESKY_ADMOB
            Advertising.Instance.ShowPrivacyOptionsForm();
#endif
        }

        void OnClickShowHideAdmin()
        {
            if (isShow)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        void Show()
        {
            isShow = true;
            Refresh();
            holder.gameObject.SetActive(true);
            Tween.UIAnchoredPositionX(container, 550, .5f, Ease.OutBack).OnComplete(() =>
            {
                iconButtonShowAdmin.sprite = iconBtnHide;
            });
        }

        void Hide()
        {
            isShow = false;
            Tween.UIAnchoredPositionX(container, 0, .5f, Ease.InBack).OnComplete(() =>
            {
                holder.gameObject.SetActive(false);
                iconButtonShowAdmin.sprite = iconBtnShow;
            });
        }

        void Init()
        {
            isShow = false;
            container.anchoredPosition = Vector2.zero;
            holder.gameObject.SetActive(false);
            iconButtonShowAdmin.sprite = iconBtnShow;
        }
    }
}