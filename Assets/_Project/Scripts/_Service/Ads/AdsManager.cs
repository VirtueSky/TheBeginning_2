using System;
using Base.Data;
using Base.Game;
using UnityEngine;
using VirtueSky.Ads;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigs;
using VirtueSky.TrackingRevenue;

namespace Base.Services
{
    [EditorIcon("icon_manager"), HideMonoScript]
    public class AdsManager : Singleton<AdsManager>
    {
        private float timePlay;

        private void Start()
        {
            ResetCounter();
        }

        public override void FixedTick()
        {
            base.FixedTick();
            if (GameManager.Instance != null && GameManager.Instance.GameState == GameState.PlayingLevel)
            {
                timePlay += Time.deltaTime;
            }
        }

        public void ResetCounter()
        {
            UserData.AdsCounter = 0;
            timePlay = 0;
        }

        bool IsEnableToShowInter()
        {
            return UserData.CurrentLevel >= RemoteData.RMC_LEVEL_TURN_ON_INTER_ADS &&
                   UserData.AdsCounter >= RemoteData.RMC_INTER_CAPPING_LEVEL &&
                   timePlay >= RemoteData.RMC_INTER_CAPPING_TIME &&
                   RemoteData.RMC_ON_OFF_INTER &&
                   !UserData.IsOffInterAdsAdministrator;
        }

        bool IsEnableToShowBanner()
        {
            return !UserData.IsTestOffBannerAdsAdministrator &&
                   RemoteData.RMC_ON_OFF_BANNER;
        }

        public bool IsRewardReady()
        {
            return Advertising.Instance.IsRewardedReady();
        }

        bool IsEnableToShowReward()
        {
            return !UserData.IsOffRewardAdsAdministrator;
        }

        public void ShowBanner()
        {
            if (IsEnableToShowBanner())
            {
                Advertising.Instance.ShowBanner();
                AppTracking.FirebaseAnalyticTrack("Show_Banner");
            }
        }

        public void HideBanner()
        {
            Advertising.Instance.HideBanner();
            AppTracking.FirebaseAnalyticTrack("Hide_Banner");
        }

        public void ShowInterstitial(Action completeCallback = null, Action displayCallback = null)
        {
            if (IsEnableToShowInter())
            {
                if (Advertising.Instance.IsInterstitialReady())
                {
                    AppTracking.FirebaseAnalyticTrack("Request_Interstitial");
                    Advertising.Instance.ShowInterstitial().OnCompleted(() =>
                    {
                        completeCallback?.Invoke();
                        AppTracking.FirebaseAnalyticTrack("Show_Interstitial_Completed");
                        ResetCounter();
                    }).OnDisplayed(displayCallback);
                }
                else
                {
                    completeCallback?.Invoke();
                    ResetCounter();
                }
            }
            else
            {
                completeCallback?.Invoke();
                ResetCounter();
            }
        }

        public void ShowRewardAds(Action completeCallback = null, Action skipCallback = null,
            Action displayCallback = null,
            Action closeCallback = null)
        {
            if (IsEnableToShowReward())
            {
                if (Advertising.Instance.IsRewardedReady())
                {
                    AppTracking.FirebaseAnalyticTrack("Request_Reward");
                    Advertising.Instance.ShowReward().OnCompleted(() =>
                    {
                        completeCallback?.Invoke();
                        AppTracking.FirebaseAnalyticTrack("Show_Reward_Completed");
                    }).OnDisplayed(displayCallback).OnClosed(closeCallback).OnSkipped(skipCallback);
                }
                else if (Advertising.Instance.IsInterstitialReady())
                {
                    Advertising.Instance.ShowInterstitial().OnCompleted(completeCallback).OnDisplayed(displayCallback)
                        .OnClosed(closeCallback)
                        .OnSkipped(skipCallback);
                }
                else
                {
                    AppTracking.FirebaseAnalyticTrack("Reward ads not ready");
                }
            }
            else
            {
                AppTracking.FirebaseAnalyticTrack("Reward ads not ready");
            }
        }
    }
}