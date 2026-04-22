using System;
using Base.Data;
using UnityEngine;
using VirtueSky.Ads;

namespace Base.Services
{
    [CreateAssetMenu(fileName = "reward_ads_wrapper", menuName = "Ads Wrapper/Reward")]
    public class RewardAdWrapper : AdWrapper
    {
        public override void Init()
        {
        }

        bool Conditions()
        {
            return Advertising.RewardAd(AdMediation.Admob).IsReady() && !UserData.IsOffRewardAdsDebug;
        }

        public void Show(Action completed = null, Action skipped = null, Action displayed = null, Action closed = null)
        {
            if (Conditions())
            {
                Advertising.RewardAd(AdMediation.Admob).Show().OnCompleted(completed).OnSkipped(skipped)
                    .OnDisplayed(adsInfo => displayed?.Invoke())
                    .OnClosed(info => closed?.Invoke());
            }
        }
    }
}