using System.Threading.Tasks;
using Base.Data;
using UnityDebugSheet.Runtime.Core.Scripts;
using VirtueSky.Ads;

namespace Base.Services
{
    public class AdsDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "Advertising Debug";

        public override Task Initialize()
        {
            AddButton("Show Banner", clicked: () => Advertising.Instance.ShowBanner());
            AddButton("Hide Banner", clicked: () => Advertising.Instance.HideBanner());
            AddButton("Show Inter", clicked: () => Advertising.Instance.ShowInterstitial());
            AddButton("Show Reward", clicked: () => Advertising.Instance.ShowReward());
            AddSwitch(UserData.IsOffInterAdsDebug, "Is Off Inter", valueChanged: b => UserData.IsOffInterAdsDebug = b,
                icon: DebugViewStatic.IconToggleDebug);
            AddSwitch(UserData.IsOffBannerAdsDebug, "Is Off Banner",
                valueChanged: b => UserData.IsOffBannerAdsDebug = b,
                icon: DebugViewStatic.IconToggleDebug);
            AddSwitch(UserData.IsOffRewardAdsDebug, "Is Off Reward",
                valueChanged: b => UserData.IsOffRewardAdsDebug = b,
                icon: DebugViewStatic.IconToggleDebug);
            return base.Initialize();
        }
    }
}