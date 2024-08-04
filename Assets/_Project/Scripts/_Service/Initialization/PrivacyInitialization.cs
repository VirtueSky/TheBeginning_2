#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using VirtueSky.Inspector;
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

        private void TrackingIosATT()
        {
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