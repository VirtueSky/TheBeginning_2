using System;
using System.Collections;
using Google.Play.Review;
using MoreMountains.NiceVibrations;
using Pancake.Monetization;
using Pancake.Threading.Tasks;
using UnityEngine;

public class PopupTest : Popup
{
    private bool isShowBanner;
    
    #region SetupInAppReview
#if UNITY_ANDROID
    private ReviewManager _reviewManager = null;
    private PlayReviewInfo _playReviewInfo = null;
#endif
    public void Start()
    {
#if UNITY_ANDROID
        StartCoroutine(IeCallInitReview());
#endif
    }

    private void RateUs()
    {
        var url = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "market://details?id=" + Application.identifier;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //url = "itms-apps://itunes.apple.com/app/id" + Application.identifier;
            url = "https://apps.apple.com/us/app/id6443935145";
        }

#if UNITY_EDITOR
        if (string.IsNullOrEmpty(url))
        {
            url = "market://details?id=" + Application.identifier;
        }
#endif
        
        Application.OpenURL(url);
        
    }

#if UNITY_ANDROID
    private IEnumerator IeCallInitReview()
    {
        if (_reviewManager == null)
        {
            _reviewManager = new ReviewManager();
        }

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
    }
    
    private IEnumerator IeCallReview()
    {
        if (_playReviewInfo == null)
        {
            yield return StartCoroutine(IeCallInitReview());
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
    
    public async void OnClickInAppReview()
    {
#if UNITY_ANDROID
        try
        {
            await UniTask.WaitUntil(() => _playReviewInfo != null);
            StartCoroutine(IeCallReview());
        }
        catch (Exception)
        {
            RateUs();
        }
#elif UNITY_IOS
        RateUs();
#endif
    }
#endif
    #endregion

    public void OnClickBtnShowBannerAds()
    {
        if (!isShowBanner)
        {
            Advertising.ShowBannerAd();
        }
        else
        {
            Advertising.HideBannerAd();
        }
        
    }

    public void OnClickBtnShowInterstitialAds()
    {
        Advertising.ShowInterstitialAd();
    }
    
    public void OnClickBtnShowRewardAds()
    {
        Advertising.ShowRewardedAd();
    }
    
    public void OnClickLightVibration()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
    
    public void OnClickMediumVibration()
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
    }
    
    public void OnClickHeavyVibration()
    {
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }

    public void OnClickBuyCoin()
    {
        
    }

    public void OnClickRemoveAds()
    {
        
    }
}
