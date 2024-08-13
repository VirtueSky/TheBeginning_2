#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigs;
using VirtueSky.Threading.Tasks;
using VirtueSky.Tracking;

namespace Base.Services
{
    [HideMonoScript]
    public class PrivacyInitialization : ServiceInitialization
    {
        public override void Initialization()
        {
            TrackingIosATT();
        }

        private async void TrackingIosATT()
        {
            await UniTask.WaitUntil(() => FirebaseRemoteConfigManager.FirebaseDependencyAvailable);
#if UNITY_IOS
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking(FirebaseTracking.TrackEventATTResult);
            }
#endif
        }
    }
}