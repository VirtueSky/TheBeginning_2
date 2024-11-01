using System.Collections;
using System.Threading.Tasks;
using Base.Data;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VirtueSky.Ads;

namespace Base.Services
{
    public class AdsDebugPage : DefaultDebugPageBase
    {
        private Sprite iconToggle;
        protected override string Title => "Advertising Debug";

        public void Init(Sprite _iconToggle)
        {
            iconToggle = _iconToggle;
        }
#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
        {
            OnInitialize();
            return base.Initialize();
        }
#else
        public override IEnumerator Initialize()
        {
            OnInitialize();
            return base.Initialize();
        }
#endif


        void OnInitialize()
        {
            AddButton("Show Banner", clicked: ShowBanner);
            AddButton("Hide Banner", clicked: HideBanner);
            AddButton("Show Inter", clicked: ShowInter);
            AddButton("Show Reward", clicked: ShowReward);
            AddSwitch(UserData.IsOffInterAdsDebug, "Is Off Inter", valueChanged: b => UserData.IsOffInterAdsDebug = b,
                icon: iconToggle);
            AddSwitch(UserData.IsOffBannerAdsDebug, "Is Off Banner",
                valueChanged: b => UserData.IsOffBannerAdsDebug = b,
                icon: iconToggle);
            AddSwitch(UserData.IsOffRewardAdsDebug, "Is Off Reward",
                valueChanged: b => UserData.IsOffRewardAdsDebug = b,
                icon: iconToggle);
        }

        void ShowBanner()
        {
            if (Application.isMobilePlatform)
            {
                Advertising.BannerAd.Show();
            }
            else
            {
                NotificationInGame.Show("Only works on mobile platform");
            }
        }

        void HideBanner()
        {
            if (Application.isMobilePlatform)
            {
                Advertising.BannerAd.HideBanner();
            }
            else
            {
                NotificationInGame.Show("Only works on mobile platform");
            }
        }

        void ShowInter()
        {
            if (Application.isMobilePlatform)
            {
                Advertising.InterstitialAd.Show();
            }
            else
            {
                NotificationInGame.Show("Only works on mobile platform");
            }
        }

        void ShowReward()
        {
            if (Application.isMobilePlatform)
            {
                Advertising.RewardAd.Show();
            }
            else
            {
                NotificationInGame.Show("Only works on mobile platform");
            }
        }
    }
}