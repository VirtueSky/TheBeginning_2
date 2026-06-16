using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;

#if VIRTUESKY_FIREBASE_REMOTECONFIG
using Firebase.Extensions;
using Firebase.RemoteConfig;
#endif

using Google.MiniJSON;
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
            RemoteDataExtensions.Storage = new RemoteConfigStorage();
            PrepareLoad();
            LoadFromPrefs();
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

            FirebaseFetchDataAsync();
#endif
        }

        #region Firebase

        private void FirebaseFetchDataAsync()
        {
#if VIRTUESKY_FIREBASE_REMOTECONFIG
            VLog.Log("Fetching data...");
            var setting = _fbRemoteConfigInstance.ConfigSettings;
            setting.MinimumFetchIntervalInMilliseconds = 0;
            _fbRemoteConfigInstance.SetConfigSettingsAsync(setting).ContinueWithOnMainThread(task =>
            {
                Task fetchTask = _fbRemoteConfigInstance.FetchAndActivateAsync();
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
                        // OPTIMIZED: Use generated methods - Zero reflection!
                        FirebaseMergeAllKeys_Optimized();

                        this.StartCoroutine(SaveRemoteConfigToPrefCoroutine());

                        EndLoad();
                        VLog.Log(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
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

                    break;
                case LastFetchStatus.Pending:
                    VLog.Log("Latest Fetch call still pending.");
                    break;
            }
#endif
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
                        (Dictionary<string, object>)Json.Deserialize(_fbRemoteConfigInstance.GetValue(k).StringValue);
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
                            valueStr = Json.Serialize(data.Value);
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
        
        public void SaveToPrefs()
        {
            RemoteDataExtensions.SaveToPrefs_Generated();

            VLog.Log("SaveToPrefs_Optimized Done - Zero reflection used!");
        }

        private IEnumerator SaveRemoteConfigToPrefCoroutine()
        {
            yield return null;
            SaveToPrefs();
        }

       
        private void LoadFromPrefs()
        {
            RemoteDataExtensions.LoadFromPrefs_Generated();

            VLog.Log("LoadFromPrefs_Optimized Done - Zero reflection used!");
        }

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
            Debug.Log(RemoteDataExtensions.ExportToString_Generated());
            OnLoaded?.Invoke();
        }
    }
}