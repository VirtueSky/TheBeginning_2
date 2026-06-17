using Base.Data;
using UnityEngine;
using VirtueSky.Ads;
using VirtueSky.RemoteConfigs;

namespace Base.Services
{
    [CreateAssetMenu(fileName = "banner_ads_wrapper", menuName = "Ads Wrapper/Banner")]
    public class BannerAdWrapper : AdWrapper
    {
        public override void Init()
        {
        }

        bool Conditions()
        {
            return Advertising.IsExist && Advertising.BannerAd() != null && Advertising.BannerAd().IsReady() && !UserData.IsOffBannerAdsDebug;
        }

        public void Show()
        {
            if (Conditions()) Advertising.BannerAd(AdMediation.Admob).Show();
        }
    }
}