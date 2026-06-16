using System;
using System.Collections.Generic;
using Base.Data;
#if VIRTUESKY_FIREBASE_ANALYTIC
using Firebase.Analytics;
#endif
using Newtonsoft.Json;
using VirtueSky.Misc;
using VirtueSky.RemoteConfigGenerated;
using VirtueSky.Utils;

namespace VirtueSky.Tracking
{
    [Serializable]
    public class AppsFlyerAttribution
    {
        public string af_status;
        public string media_source;
        public string campaign;
        public string adset;
        public string ad;
    }

    public static partial class TrackUserProperty
    {
        private static Dictionary<string, List<object>> CacheUserProperty = new Dictionary<string, List<object>>();

        public static void UaProperty(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                SetProperty("ua_network", "Unattributed");
                SetProperty("ua_campaign", "Unattributed");
                SetProperty("ua_adgroup", "Unattributed");
                SetProperty("ua_creative", "Unattributed");
            }
            else
            {
                var attribution = JsonConvert.DeserializeObject<AppsFlyerAttribution>(data);
                if (attribution.af_status == "Non-organic")
                {
                    SetProperty("ua_network", attribution.media_source ?? "Unavailable");
                    SetProperty("ua_campaign", attribution.campaign ?? "Unavailable");
                    SetProperty("ua_adgroup", attribution.adset ?? "Unavailable");
                    SetProperty("ua_creative", attribution.ad ?? "Unavailable");
                }
                else
                {
                    SetProperty("ua_network", attribution.media_source ?? "Organic");
                    SetProperty("ua_campaign", attribution.campaign ?? "Organic");
                    SetProperty("ua_adgroup", attribution.adset ?? "Organic");
                    SetProperty("ua_creative", attribution.ad ?? "Organic");
                }
            }
        }

        
        
        private static void SetProperty(string name, object value)
        {
            
            if (CacheUserProperty.TryGetValue(name, out var valueList))
            {
                valueList.Add(value);
            }
            else
            {
                CacheUserProperty[name] = new List<object> { value };
            }

            if (RemoteConfig.IsFirebaseAppDependencyStatusAvailable)
            {
                DebugTracking(name, CacheUserProperty);
#if VIRTUESKY_FIREBASE_ANALYTIC
                foreach (var keyValuePair in CacheUserProperty)
                {
                    var nameCache = keyValuePair.Key;
                    var listValue = keyValuePair.Value;
                    foreach (var vl in listValue)
                    {
                        FirebaseAnalytics.SetUserProperty(nameCache, vl.ToString());
                    }
                }
#endif
                CacheUserProperty.Clear();
            }
        }

        private static void DebugTracking(string eventName, Dictionary<string, List<object>> param = null)
        {
            string log = "FirebaseProperty: " + eventName;
            if (param != null && param.Count > 0)
            {
                log += " { ";
                foreach (var p in param)
                {
                    if (!string.IsNullOrEmpty(p.Key) && p.Value != null)
                    {
                        log += $" {p.Key} : \"{string.Join(",", p.Value)}\";";
                    }
                }

                log += " }";
            }

            VLog.Log(log.SetColor(CustomColor.Orange));
            if (param != null && param.Count > 25)
            {
                VLog.LogError($"FirebaseProperty: \"{eventName}\" has {param.Count} parameters. {param.Count - 25} parameter(s) will not be sent.");
            }
        }
    }
}