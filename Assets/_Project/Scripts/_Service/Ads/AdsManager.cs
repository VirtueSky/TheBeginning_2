using System;
using Base.Data;
using Base.Game;
using UnityEngine;
using VirtueSky.Ads;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigs;
using VirtueSky.Tracking;

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
            return Advertising.InterstitialAd.IsReady() &&
                   UserData.CurrentLevel >= RemoteData.RMC_LEVEL_TURN_ON_INTER_ADS &&
                   UserData.AdsCounter >= RemoteData.RMC_INTER_CAPPING_LEVEL &&
                   timePlay >= RemoteData.RMC_INTER_CAPPING_TIME &&
                   RemoteData.RMC_ON_OFF_INTER &&
                   !UserData.IsOffInterAdsDebug;
        }

        bool IsEnableToShowBanner()
        {
            return !UserData.IsOffBannerAdsDebug &&
                   RemoteData.RMC_ON_OFF_BANNER;
        }


        bool IsEnableToShowReward()
        {
            return Advertising.RewardAd.IsReady() && !UserData.IsOffRewardAdsDebug;
        }

        public void ShowBanner()
        {
            if (IsEnableToShowBanner())
            {
                Advertising.BannerAd.Show();
                FirebaseTracking.TrackEvent("Show_Banner");
            }
        }

        public void HideBanner()
        {
            Advertising.BannerAd.HideBanner();
            FirebaseTracking.TrackEvent("Hide_Banner");
        }

        public void ShowInterstitial(Action completeCallback = null, Action displayCallback = null)
        {
            if (IsEnableToShowInter())
            {
                FirebaseTracking.TrackEvent("Request_Interstitial");
                Advertising.InterstitialAd.Show().OnCompleted(() =>
                {
                    completeCallback?.Invoke();
                    FirebaseTracking.TrackEvent("Show_Interstitial_Completed");
                    ResetCounter();
                }).OnDisplayed(displayCallback);
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
                FirebaseTracking.TrackEvent("Request_Reward");
                Advertising.RewardAd.Show().OnCompleted(() =>
                {
                    completeCallback?.Invoke();
                    FirebaseTracking.TrackEvent("Show_Reward_Completed");
                }).OnDisplayed(displayCallback).OnClosed(closeCallback).OnSkipped(skipCallback);
            }
            else
            {
                AppTracking.FirebaseAnalyticTrack("Reward ads not ready");
            }
        }
    }
}