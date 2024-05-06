using Base.Global;
using VirtueSky.DataStorage;

namespace Base.Data
{
    public partial struct UserData
    {
        public static bool IsTestingAdministrator
        {
            get => GameData.Get(Constant.IS_TESTING, false);
            set
            {
                GameData.Set(Constant.IS_TESTING, value);
                Observer.IsTestingChanged?.Invoke();
            }
        }

        public static bool IsOffInterAdsAdministrator
        {
            get => GameData.Get(Constant.IS_OFF_INTER_ADS_ADMIN, false);
            set => GameData.Set(Constant.IS_OFF_INTER_ADS_ADMIN, value);
        }

        public static bool IsTestOffBannerAdsAdministrator
        {
            get => GameData.Get(Constant.IS_OFF_BANNER_ADS_ADMIN, false);
            set => GameData.Set(Constant.IS_OFF_BANNER_ADS_ADMIN, value);
        }

        public static bool IsOffRewardAdsAdministrator
        {
            get => GameData.Get(Constant.IS_OFF_REWARD_ADS_ADMIN, false);
            set => GameData.Set(Constant.IS_OFF_REWARD_ADS_ADMIN, value);
        }

        public static bool IsOffUIAdministrator
        {
            get => GameData.Get(Constant.IS_OFF_UI_ADMIN, false);
            set => GameData.Set(Constant.IS_OFF_UI_ADMIN, value);
        }
    }
}