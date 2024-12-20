using System;
using Base.Data;
using Base.Game;
using Base.Levels;
using UnityEngine;
using VirtueSky.Ads;
using VirtueSky.Core;
using VirtueSky.Misc;
using VirtueSky.RemoteConfigs;
using VirtueSky.Utils;

namespace Base.Services
{
    [CreateAssetMenu(fileName = "inter_ads_wrapper", menuName = "Ads Wrapper/Inter")]
    public class InterAdWrapper : AdWrapper
    {
        private float timeAdsPlay;
        private int adsCounter = 0;

        public override void Init()
        {
            App.SubTick(OnUpdate);
            GameManager.OnWinLevel += OnWinLevel;
            GameManager.OnLoseLevel += OnLoseLevel;
        }


        void OnUpdate()
        {
            if (GameManager.Instance != null && GameManager.Instance.GameState == GameState.PlayingLevel)
            {
                timeAdsPlay += Time.deltaTime;
            }
        }

        private void OnLoseLevel(Level level)
        {
            adsCounter++;
        }

        private void OnWinLevel(Level level)
        {
            adsCounter++;
        }

        private bool Conditions()
        {
            return Advertising.InterstitialAd.IsReady() &&
                   UserData.CurrentLevel >= RemoteData.LEVEL_TURN_ON_INTER_ADS &&
                   adsCounter >= RemoteData.INTER_CAPPING_LEVEL &&
                   timeAdsPlay >= RemoteData.INTER_CAPPING_TIME && RemoteData.ON_OFF_INTER &&
                   !UserData.IsOnOffInterAdsDebug;
        }

        public void Show(Action completed = null, Action displayed = null)
        {
            if (Conditions())
            {
                Advertising.InterstitialAd.Show().OnCompleted(() =>
                {
                    completed?.Invoke();
                    adsCounter = 0;
                    timeAdsPlay = 0;
                }).OnDisplayed(displayed);
            }
            else
            {
                completed?.Invoke();
            }
        }
    }
}