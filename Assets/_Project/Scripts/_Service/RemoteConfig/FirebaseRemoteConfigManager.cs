using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Inspector;
#if VIRTUESKY_FIREBASE
using Firebase;
using Firebase.Extensions;
#endif

#if VIRTUESKY_FIREBASE_REMOTECONFIG
using Firebase.RemoteConfig;
using VirtueSky.Threading.Tasks;
#endif

namespace Base.Services
{
    [EditorIcon("icon_controller"), HideMonoScript]
    public class FirebaseRemoteConfigManager : Singleton<FirebaseRemoteConfigManager>
    {
#if VIRTUESKY_FIREBASE
        [ReadOnly, SerializeField] private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
#endif
        [SerializeField] private bool isSetupDefaultData = true;
        [SerializeField] private List<FirebaseRemoteConfigData> listRemoteConfigData;

        public List<FirebaseRemoteConfigData> ListRemoteConfigData => listRemoteConfigData;


        private bool isFetchRemoteConfigCompleted = false;
        public bool IsFetchRemoteConfigCompleted => isFetchRemoteConfigCompleted;

#if VIRTUESKY_FIREBASE
        private void Start()
        {
            isFetchRemoteConfigCompleted = false;
            if (isSetupDefaultData)
            {
                foreach (var remoteConfigData in listRemoteConfigData)
                {
                    remoteConfigData.SetupDataDefault();
                }
            }

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
#if VIRTUESKY_FIREBASE_REMOTECONFIG
                    FetchDataAsync();
#endif
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " +
                                   dependencyStatus);
                }
            });
        }
#endif

#if VIRTUESKY_FIREBASE_REMOTECONFIG && VIRTUESKY_FIREBASE
        public Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance
                .FetchAsync(TimeSpan.Zero);
            if (fetchTask.IsCanceled)
            {
                Debug.Log("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("Fetch completed successfully!");
            }

            return fetchTask.ContinueWithOnMainThread(tast =>
            {
                var info = FirebaseRemoteConfig.DefaultInstance.Info;
                if (info.LastFetchStatus == LastFetchStatus.Success)
                {
                    FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(
                        task =>
                        {
                            Debug.Log(String.Format(
                                "Remote data loaded and ready (last fetch time {0}).",
                                info.FetchTime));
                            foreach (var remoteConfigData in listRemoteConfigData)
                            {
                                if (!string.IsNullOrEmpty(remoteConfigData.key.ToString()))
                                {
                                    remoteConfigData.SetupData(FirebaseRemoteConfig.DefaultInstance
                                        .GetValue(remoteConfigData.key.ToString()));
                                }
                            }

                            isFetchRemoteConfigCompleted = true;
                        });

                    Debug.Log("<color=Green>Firebase Remote Config Fetching completed!</color>");
                }
                else
                {
                    Debug.Log("Fetching data did not completed!");
                }
            });
        }
#endif
    }
}