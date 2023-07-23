using System;
using Pancake.Monetization;
using Pancake.Threading.Tasks;
using UnityEngine;

public class AdsController : SingletonDontDestroy<AdsController>
{
    private int _adsCounter;
    private float _timePlay;
    public bool IsInitialized => Advertising.IsInitialized;

    protected override void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Observer.WinLevel += OnAdsCount;
        Observer.LoseLevel += OnAdsCount;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameState == GameState.PlayingGame)
        {
            _timePlay += Time.deltaTime;
        }
    }

    private void OnAdsCount(Level level)
    {
        _adsCounter++;
    }

    public async void Initialize()
    {
        await UniTask.WaitUntil(() => Advertising.ApplovinAdClient.IsInitialized);
        Advertising.ApplovinAdClient.OnRewardedAdRevenuePaid += OnRevenuePaid;
        Advertising.ApplovinAdClient.OnBannerAdRevenuePaid += OnRevenuePaid;
        Advertising.ApplovinAdClient.OnInterstitialAdRevenuePaid += OnRevenuePaid;
    }

    private static void OnRevenuePaid(MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        // send ad revenue info to Adjust
        // AdjustAdRevenue adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        // adRevenue.setRevenue(adInfo.Revenue, "USD");
        // adRevenue.setAdRevenueNetwork(adInfo.NetworkName);
        // adRevenue.setAdRevenuePlacement(adInfo.Placement);
        // adRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
        // Adjust.trackAdRevenue(adRevenue);
    }

    private bool IsEnableToShowInter()
    {
        if (Data.CurrentLevel > Data.LevelTurnOnInterstitial &&
            _adsCounter >= Data.CounterNumbBetweenTwoInterstitial &&
            _timePlay >= Data.TimeWinBetweenTwoInterstitial && !Data.IsOffInterAds)
        {
            return true;
        }

        return false;
    }

    public void ShowInterstitial(Action completeCallback, Action displayCallback = null)
    {
        if (IsEnableToShowInter())
        {
            AdsManager.ShowInterstitial(completeCallback, displayCallback);
        }
        else
        {
            completeCallback?.Invoke();
        }
    }

    public void ShowRewardAds(Action completeCallback, Action displayCallback = null,
        Action closeCallback = null, Action skipCallback = null)
    {
        AdsManager.ShowRewardAds(completeCallback, displayCallback, closeCallback, skipCallback);
    }

    public static bool IsSufficientConditionToShowBanner()
    {
        // if (IAPManager.Instance.IsPurchased(Constant.ANDROID_IAP_REMOVE_ADS) || IAPManager.Instance.IsPurchased(Constant.IOS_IAP_REMOVE_ADS))
        // {
        //     return false;
        // }
        //
        // return true;
        return false;
    }

    public static void ShowBanner()
    {
        if (IsSufficientConditionToShowBanner())
        {
            //FirebaseManager.OnShowBanner();
            Advertising.ShowBannerAd();
        }
    }

    public static void HideBanner()
    {
        Advertising.HideBannerAd();
    }
}