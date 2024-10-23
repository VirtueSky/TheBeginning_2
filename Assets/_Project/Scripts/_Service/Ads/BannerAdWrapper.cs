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
            return Advertising.BannerAd.IsReady() && !UserData.IsOffBannerAdsDebug && RemoteData.RMC_ON_OFF_BANNER;
        }

        public void Show()
        {
            if (Conditions()) Advertising.BannerAd.Show();
        }
    }
}