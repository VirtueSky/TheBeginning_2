using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VIRTUESKY_FIREBASE
using Firebase;
#endif
#if VIRTUESKY_FIREBASE_REMOTECONFIG
using Firebase.Extensions;
using Firebase.RemoteConfig;

#endif

using Newtonsoft.Json;
using RemoteConfigGenerator;
using VirtueSky.Pattern;
using VirtueSky.Utils;
using Task = System.Threading.Tasks.Task;


namespace VirtueSky.RemoteConfigGenerated
{
    public class RemoteConfig : Singleton<RemoteConfig>
    {
        public event Action OnRemoteConfigLoaded;
#if VIRTUESKY_FIREBASE_REMOTECONFIG
        private FirebaseRemoteConfig _fbRemoteConfigInstance;
#endif

        public bool enableRemoteSync = true;
        public static bool IsLoaded = false;
        public static bool IsFirebaseAppDependencyStatusAvailable { get; private set; } = false;

        public static event Action OnLoaded;

        protected void Awake()
        {
            PrepareLoad();
            IsLoaded = false;
#if VIRTUESKY_FIREBASE
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    try
                    {
                        LoadRemoteConfig();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.ToString());
                    }

                    IsFirebaseAppDependencyStatusAvailable = true;
                }
                else
                {
                    Debug.LogError(String.Format("Could not resolve all Firebase dependencies: {0}", task.Result));
                }
            });
#endif

        }

        public void LoadRemoteConfig()
        {
#if VIRTUESKY_FIREBASE_REMOTECONFIG    
            _fbRemoteConfigInstance = FirebaseRemoteConfig.DefaultInstance;
            if (!enableRemoteSync)
            {
                EndLoad();
                return;
            }

            ActivateCachedValuesAndLoad();
#endif
        }

        #region Firebase

        private void ActivateCachedValuesAndLoad()
        {
#if VIRTUESKY_FIREBASE_REMOTECONFIG
            _fbRemoteConfigInstance.ActivateAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    VLog.Log("Activate cached values canceled.");
                }
                else if (task.IsFaulted)
                {
                    VLog.Log("Activate cached values encountered an error.");
                }

                if (TryApplyActivatedValues())
                {
                    VLog.Log("Activated cached Remote Config values from a previous session. Waiting for remote fetch before EndLoad.");
                }
                else
                {
                    VLog.Log("No cached Remote Config values available. Using in-app defaults until remote fetch completes.");
                }

                FirebaseFetchDataAsync();
            });
#endif
        }

        private void FirebaseFetchDataAsync()
        {
#if VIRTUESKY_FIREBASE_REMOTECONFIG
            VLog.Log("Fetching data...");
            var setting = _fbRemoteConfigInstance.ConfigSettings;
            setting.MinimumFetchIntervalInMilliseconds = 0;
            _fbRemoteConfigInstance.SetConfigSettingsAsync(setting).ContinueWithOnMainThread(task =>
            {
                Task fetchTask = _fbRemoteConfigInstance.FetchAsync();
                fetchTask.ContinueWithOnMainThread(FirebaseFetchComplete);
            });
#endif
        }

        void FirebaseFetchComplete(Task fetchTask)
        {
            if (IsLoaded)
            {
                return;
            }

            if (fetchTask.IsCanceled)
            {
                VLog.Log("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                VLog.Log("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                VLog.Log("Fetch completed successfully!");
            }
#if VIRTUESKY_FIREBASE_REMOTECONFIG
            var info = _fbRemoteConfigInstance.Info;
            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    _fbRemoteConfigInstance.ActivateAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                        {
                            VLog.Log("Activate fetched values canceled.");
                        }
                        else if (task.IsFaulted)
                        {
                            VLog.Log("Activate fetched values encountered an error.");
                        }

                        if (TryApplyActivatedValues())
                        {
                            EndLoad();
                            VLog.Log(String.Format("Remote data loaded and ready with latest fetched values (last fetch time {0}).", info.FetchTime));
                        }
                        else
                        {
                            VLog.Log("Fetch succeeded but no activated Remote Config keys were available. Using current values.");
                            EndLoad();
                        }
                    });
                    break;
                case LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case FetchFailureReason.Error:
                            VLog.Log("Fetch failed for unknown reason");
                            break;
                        case FetchFailureReason.Throttled:
                            VLog.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }

                    if (!IsLoaded)
                    {
                        VLog.Log("Continuing startup with the currently active Remote Config values or in-app defaults.");
                        EndLoad();
                    }

                    break;
                case LastFetchStatus.Pending:
                    VLog.Log("Latest Fetch call still pending.");

                    if (!IsLoaded)
                    {
                        EndLoad();
                    }

                    break;
            }
#endif
        }

        private bool TryApplyActivatedValues()
        {
#if VIRTUESKY_FIREBASE_REMOTECONFIG
            foreach (var _ in _fbRemoteConfigInstance.Keys)
            {
                FirebaseMergeAllKeys_Optimized();
                return true;
            }
#endif
            return false;
        }
        
        public void FirebaseMergeAllKeys_Optimized()
        {
#if VIRTUESKY_FIREBASE_REMOTECONFIG
            IEnumerable<string> keys = _fbRemoteConfigInstance.Keys;

            foreach (string k in keys)
            {
                // Handle nested Settings keys (e.g., "AdSettings", "ShopSettings")
                if (k.Contains("Settings"))
                {
                    Dictionary<string, object> jsonDict =
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(_fbRemoteConfigInstance.GetValue(k).StringValue);
                    MergeNestedKeys_Optimized(jsonDict, k.Replace("Settings", ""));
                    continue;
                }
                
                if (RemoteDataExtensions.FieldSetterLookup.TryGetValue(k, out Action<string> setter))
                {
                    var configValue = _fbRemoteConfigInstance.GetValue(k);
                    setter.Invoke(configValue.StringValue);
                    continue;
                }

                // Alternative: Use generated SetFieldValue_Generated for type-safe ConfigValue handling
                var configValueAlt = _fbRemoteConfigInstance.GetValue(k);
                bool handled = RemoteDataExtensions.SetFieldValue_Generated(k, configValueAlt);

                if (!handled)
                {
#if UNITY_EDITOR
                    VLog.LogWarning(
                        $"Key '{k}' from Firebase not found in RemoteData class. Add [RemoteConfigField] attribute to handle it.");
#endif
                }

            }
#endif
            VLog.Log("FirebaseMergeAllKeys_Optimized completed - Zero reflection used!");
        }
        
        private void MergeNestedKeys_Optimized(Dictionary<string, object> jsonDict, string keyPrefix)
        {
            foreach (KeyValuePair<string, object> data in jsonDict)
            {
                string fullKey = keyPrefix + data.Key;

                try
                {
                    // OPTIMIZED: Use generated FieldSetterLookup - no reflection!
                    if (RemoteDataExtensions.FieldSetterLookup.TryGetValue(fullKey, out Action<string> setter))
                    {
                        string valueStr;
                        if (data.Value is string)
                        {
                            valueStr = data.Value.ToString();
                        }
                        else
                        {
                            valueStr = JsonConvert.SerializeObject(data.Value);
                        }

                        setter.Invoke(valueStr);

#if UNITY_EDITOR
                        VLog.Log($"[Optimized] Updated {fullKey}: {valueStr}");
#endif
                    }
                    else
                    {
#if UNITY_EDITOR
                        VLog.LogWarning($"Key {fullKey} from Firebase not found in RemoteData class!");
#endif
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR
                    VLog.LogWarning($"Invalid key: {fullKey}:{data.Value} - {ex.Message}");
#endif
                }
            }
        }

        #endregion
        
        /// <summary>
        /// Reset loaded flag
        /// </summary>
        public void Reset()
        {
            IsLoaded = false;
        }
        
        public string ExportToString()
        {
            // OPTIMIZED: Use generated method - no reflection!
            return RemoteDataExtensions.ExportToString_Generated();
        }

        /// <summary>
        /// Prepare default values before loading
        /// </summary>
        private void PrepareLoad()
        {
        }

        /// <summary>
        /// Remote config load completed - Apply game-specific configurations
        /// </summary>
        public void EndLoad()
        {
            if (IsLoaded) return;
            IsLoaded = true;
            Debug.Log(ExportToString());
            OnLoaded?.Invoke();
        }
    }
}
