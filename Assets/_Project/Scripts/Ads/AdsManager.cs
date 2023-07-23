using System;
using Pancake.Monetization;

public class AdsManager
{
    public static void ShowInterstitial(Action completeCallback, Action displayCallback = null)
    {
        if (Advertising.IsInterstitialAdReady())
        {
            Advertising.ShowInterstitialAd().OnDisplayed(() =>
                {
                    displayCallback?.Invoke();
                })
                .OnCompleted(() =>
                {
                    completeCallback?.Invoke();
                });
        }
        else
        {
            completeCallback?.Invoke();
        }
    }

    public static void ShowRewardAds(Action completeCallback, Action displayCallback = null,
        Action closeCallback = null, Action skipCallback = null)
    {
        if (Advertising.IsRewardedAdReady())
        {
            Advertising.ShowRewardedAd().OnDisplayed(() =>
            {
                displayCallback?.Invoke();
            })
            .OnClosed(() =>
            {
                closeCallback?.Invoke();
            }).OnSkipped(() =>
            {
                skipCallback?.Invoke();
            })
            .OnCompleted(() =>
            {
                completeCallback?.Invoke();
            });
        }
    }

    public static void ShowBanner()
    {
        Advertising.ShowBannerAd();
    }

    public static void HideBanner()
    {
        Advertising.HideBannerAd();
    }
}