using System;
using Firebase.RemoteConfig;
using UnityEngine;
using VirtueSky.DataStorage;
using VirtueSky.Inspector;

namespace Base.Services
{
    [Serializable]
    public class FirebaseRemoteConfigData
    {
        public KeyFirebaseRemoteConfig key;
        public TypeRemoteConfigData typeRemoteConfigData;

        [ShowIf(nameof(typeRemoteConfigData), TypeRemoteConfigData.StringData)]
        public string defaultValueString;

        [ShowIf(nameof(typeRemoteConfigData), TypeRemoteConfigData.StringData)] [SerializeField, ReadOnly]
        private string resultValueString;

        [ShowIf(nameof(typeRemoteConfigData), TypeRemoteConfigData.BooleanData)]
        public bool defaultValueBool;

        [ShowIf(nameof(typeRemoteConfigData), TypeRemoteConfigData.BooleanData)] [SerializeField, ReadOnly]
        private bool resultValueBool;

        [ShowIf(nameof(typeRemoteConfigData), TypeRemoteConfigData.IntData)]
        public int defaultValueInt;

        [ShowIf(nameof(typeRemoteConfigData), TypeRemoteConfigData.IntData)] [SerializeField, ReadOnly]
        private int resultValueInt;


        public void SetupDataDefault()
        {
            switch (typeRemoteConfigData)
            {
                case TypeRemoteConfigData.StringData:
                    GameData.Set(key.ToString(), defaultValueString);
                    break;
                case TypeRemoteConfigData.BooleanData:
                    GameData.Set(key.ToString(), defaultValueBool);
                    break;
                case TypeRemoteConfigData.IntData:
                    GameData.Set(key.ToString(), defaultValueInt);
                    break;
            }

            Debug.Log($"<color=Green>Setup default data remote config completed</color>");
        }
#if VIRTUESKY_FIREBASE_REMOTECONFIG
        public void SetupData(ConfigValue result)
        {
            switch (typeRemoteConfigData)
            {
                case TypeRemoteConfigData.StringData:
                    if (result.Source == ValueSource.RemoteValue)
                    {
                        GameData.Set(key.ToString(), result.StringValue);
                    }

                    resultValueString = GameData.Get<string>(key.ToString());
                    Debug.Log($"<color=Green>{key}: {resultValueString}</color>");
                    break;
                case TypeRemoteConfigData.BooleanData:
                    if (result.Source == ValueSource.RemoteValue)
                    {
                        GameData.Set(key.ToString(), result.BooleanValue);
                    }

                    resultValueBool = GameData.Get<bool>(key.ToString());
                    Debug.Log($"<color=Green>{key}: {resultValueBool}</color>");
                    break;
                case TypeRemoteConfigData.IntData:
                    if (result.Source == ValueSource.RemoteValue)
                    {
                        GameData.Set(key.ToString(), int.Parse(result.StringValue));
                    }

                    resultValueInt = GameData.Get<int>(key.ToString());
                    Debug.Log($"<color=Green>{key}: {resultValueInt}</color>");
                    break;
            }
        }
#endif


        public T GetValue<T>()
        {
            return GameData.Get<T>(key.ToString());
        }
    }

    public enum TypeRemoteConfigData
    {
        StringData,
        BooleanData,
        IntData
    }
}