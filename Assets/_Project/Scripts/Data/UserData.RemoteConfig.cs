using Base.Services;

namespace Base.Data
{
    public partial struct UserData
    {
        public T GetRemoteConfigData<T>(KeyFirebaseRemoteConfig key)
        {
            return FirebaseRemoteConfigManager.Instance.GetData(key).GetValue<T>();
        }
    }
}