using RemoteConfigGenerator;
using VirtueSky.DataStorage;

namespace VirtueSky.RemoteConfigGenerated {
    public class RemoteConfigStorage : IRemoteConfigStorage {
        public void SetInt(string key, int value) {
            GameData.Set<int>(key, value);
        }

        public int GetInt(string key, int defaultValue) {
            return GameData.Get<int>(key, defaultValue);
        }

        public void SetFloat(string key, float value) {
            GameData.Set<float>(key, value);
        }

        public float GetFloat(string key, float defaultValue) {
            return GameData.Get<float>(key, defaultValue);
        }

        public void SetString(string key, string value) {
            GameData.Set<string>(key, value);
        }

        public string GetString(string key, string defaultValue) {
            return GameData.Get<string>(key, defaultValue);
        }

        public void SetBool(string key, bool value) {
            GameData.Set<bool>(key, value);
        }

        public bool GetBool(string key, bool defaultValue) {
            return GameData.Get<bool>(key, defaultValue);
        }

        public void SetLong(string key, long value) {
            GameData.Set<long>(key, value);
        }

        public long GetLong(string key, long defaultValue) {
            return GameData.Get<long>(key, defaultValue);
        }

        public void Save() {
            GameData.Save();
        }
    }
}