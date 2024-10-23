using UnityEngine;
using VirtueSky.Inspector;

namespace Base.Services
{
    [HideMonoScript]
    public class AdsInitialization : ServiceInitialization
    {
        [SerializeField] private BannerAdWrapper bannerAdWrapper;
        [SerializeField] private InterAdWrapper interAdWrapper;
        [SerializeField] private RewardAdWrapper rewardAdWrapper;

        public override void Initialization()
        {
            bannerAdWrapper.Init();
            interAdWrapper.Init();
            rewardAdWrapper.Init();
        }
    }
}