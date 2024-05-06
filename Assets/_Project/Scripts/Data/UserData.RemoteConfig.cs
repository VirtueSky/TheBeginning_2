using Base.Services;
using VirtueSky.DataStorage;

namespace Base.Data
{
    public partial struct UserData
    {
        public static T GetRemoteConfigData<T>(KeyFirebaseRemoteConfig key)
        {
            return GameData.Get<T>(key.ToString());
        }
    }
}