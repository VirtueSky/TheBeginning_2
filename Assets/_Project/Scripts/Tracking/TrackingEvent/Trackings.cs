using System;
using System.Collections.Generic;
#if VIRTUESKY_FIREBASE_ANALYTIC
using Firebase.Analytics;
#endif
using VirtueSky.Misc;
using VirtueSky.RemoteConfigGenerated;
using VirtueSky.Utils;

namespace VirtueSky.Tracking
{
    public static partial class Trackings
    {
        private static Queue<KeyValuePair<string, Firebase.Analytics.Parameter[]>> queueAnaData =
            new Queue<KeyValuePair<string, Firebase.Analytics.Parameter[]>>(50);

        /// <summary>
        /// Track multiple param
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="param"></param>
        public static void Track(string eventName, Dictionary<string, object> param = null)
        {
            try
            {
                DebugTracking(eventName, param);
#if VIRTUESKY_FIREBASE_ANALYTIC
                if (queueAnaData.Count < 50)
                {
                    List<Parameter> parameters = new List<Parameter>();
                    if (param is { Count: > 0 })
                    {
                        foreach (var kvp in param)
                        {
                            if (string.IsNullOrEmpty(kvp.Key) || kvp.Value == null) continue;
                            if (kvp.Value is double)
                            {
                                parameters.Add(new Parameter(kvp.Key, (double)kvp.Value));
                            }
                            else if (kvp.Value is float)
                            {
                                parameters.Add(new Parameter(kvp.Key, (float)kvp.Value));
                            }
                            else if (kvp.Value is int)
                            {
                                parameters.Add(new Parameter(kvp.Key, (int)kvp.Value));
                            }
                            else if (kvp.Value is long)
                            {
                                parameters.Add(new Parameter(kvp.Key, (long)kvp.Value));
                            }
                            else if (!string.IsNullOrEmpty(kvp.Value.ToString()))
                            {
                                parameters.Add(new Parameter(kvp.Key, kvp.Value.ToString()));
                            }
                        }

                        queueAnaData.Enqueue(new KeyValuePair<string, Parameter[]>(eventName, parameters.ToArray()));
                    }
                }

                if (RemoteConfig.IsFirebaseAppDependencyStatusAvailable)
                {
                    while (queueAnaData.Count > 0)
                    {
                        var eventTracking = queueAnaData.Dequeue();
                        FirebaseAnalytics.LogEvent(eventTracking.Key, eventTracking.Value);
                    }
                }      
#endif

            }
            catch (Exception e)
            {
                VLog.LogException(e);
            }
        }

        /// <summary>
        /// Track 1 param
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public static void Track(string eventName, string parameterName, string parameterValue)
        {
            Track(eventName, new Dictionary<string, object> { { parameterName, parameterValue } });
        }

        public static bool IsShowDebugTracking()
        {
            return true;
        }

        private static void DebugTracking(string eventName, Dictionary<string, object> param = null)
        {
            if (!IsShowDebugTracking()) return;
            string log = "FirebaseEvent: " + eventName;
            if (param != null && param.Count > 0)
            {
                log += " { ";
                foreach (var p in param)
                {
                    if (!string.IsNullOrEmpty(p.Key) && p.Value != null)
                    {
                        log += $" {p.Key} : \"{p.Value}\";";
                        ;
                    }
                }

                log += " }";
            }

            VLog.Log(log.SetColor(CustomColor.Orange));
            if (param != null && param.Count > 25)
            {
                VLog.LogError($"FirebaseEvent: \"{eventName}\" has {param.Count} parameters. {param.Count - 25} parameter(s) will not be sent.");
            }
        }


        public static string ToStringParamLower(this TrackParam param) => param.ToString().ToLower();
        public static string ToStringParam(this TrackParam param) => param.ToString();
        public static string ToStringName(this TrackName name) => name.ToString();
        public static string ToStringNameLower(this TrackName name) => name.ToString().ToLower();
        public static string ToStringValue(this TrackValue value) => value.ToString();
        public static string ToStringValueLower(this TrackValue value) => value.ToString().ToLower();
    }
}