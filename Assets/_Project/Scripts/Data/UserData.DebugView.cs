using Base.Global;
using VirtueSky.DataStorage;
using Virtuesky.Events;

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
                EventName.IsTestingChanged.Raise();
            }
        }

        public static bool IsOnOffInterAdsDebug
        {
            get => GameData.Get(Constant.IS_OFF_INTER_ADS_ADMIN, true);
            set => GameData.Set(Constant.IS_OFF_INTER_ADS_ADMIN, value);
        }

        public static bool IsOnOffBannerAdsDebug
        {
            get => GameData.Get(Constant.IS_OFF_BANNER_ADS_ADMIN, true);
            set => GameData.Set(Constant.IS_OFF_BANNER_ADS_ADMIN, value);
        }

        public static bool IsOnOffRewardAdsDebug
        {
            get => GameData.Get(Constant.IS_OFF_REWARD_ADS_ADMIN, true);
            set => GameData.Set(Constant.IS_OFF_REWARD_ADS_ADMIN, value);
        }

        public static bool IsOnOffUIDebug
        {
            get => GameData.Get(Constant.IS_OFF_UI_ADMIN, true);
            set
            {
                GameData.Set(Constant.IS_OFF_UI_ADMIN, value);
                EventName.OnOffUIChanged.Raise(value);
            }
        }
    }
}