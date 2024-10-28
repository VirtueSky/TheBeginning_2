#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigs;
using Cysharp.Threading.Tasks;
using VirtueSky.Core;
using VirtueSky.Tracking;

namespace Base.Services
{
    [HideMonoScript]
    public class PrivacyInitialization : ServiceInitialization
    {
        public override void Initialization()
        {
            RequireTracking();
        }

        private void RequireTracking()
        {
#if UNITY_IOS
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking(CallbackTracking);
            }
            else
            {
                AppTracking.StartTrackingAdjust();
                AppTracking.StartTrackingAppsFlyer();
            }
#else
            AppTracking.StartTrackingAdjust();
            AppTracking.StartTrackingAppsFlyer();
#endif
        }

        private void CallbackTracking(int status)
        {
            App.RunOnMainThread(() =>
            {
                AppTracking.StartTrackingAdjust();
                AppTracking.StartTrackingAppsFlyer();
                TrackingAttFirebase(status);
            });
        }

        async void TrackingAttFirebase(int status)
        {
            await UniTask.WaitUntil(() => FirebaseRemoteConfigManager.FirebaseDependencyAvailable);
            AppTracking.TrackEventATTResult(status);
        }
    }
}