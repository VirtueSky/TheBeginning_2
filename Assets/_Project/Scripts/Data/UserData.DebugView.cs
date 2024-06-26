using Base.Global;
using VirtueSky.DataStorage;

namespace Base.Data
{
    public partial struct UserData
    {
        public static bool IsTestingDebug
        {
            get => GameData.Get(Constant.IS_TESTING, false);
            set
            {
                GameData.Set(Constant.IS_TESTING, value);
                Observer.IsTestingChanged?.Invoke();
            }
        }

        public static bool IsOffInterAdsDebug
        {
            get => GameData.Get(Constant.IS_OFF_INTER_ADS_ADMIN, false);
            set => GameData.Set(Constant.IS_OFF_INTER_ADS_ADMIN, value);
        }

        public static bool IsOffBannerAdsDebug
        {
            get => GameData.Get(Constant.IS_OFF_BANNER_ADS_ADMIN, false);
            set => GameData.Set(Constant.IS_OFF_BANNER_ADS_ADMIN, value);
        }

        public static bool IsOffRewardAdsDebug
        {
            get => GameData.Get(Constant.IS_OFF_REWARD_ADS_ADMIN, false);
            set => GameData.Set(Constant.IS_OFF_REWARD_ADS_ADMIN, value);
        }

        public static bool IsOffUIDebug
        {
            get => GameData.Get(Constant.IS_OFF_UI_ADMIN, false);
            set
            {
                GameData.Set(Constant.IS_OFF_UI_ADMIN, value);
                Observer.OffUIChanged?.Invoke(value);
            }
        }
    }
}