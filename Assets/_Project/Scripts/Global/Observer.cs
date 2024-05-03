using System;

namespace Base.Global
{
    public static class Observer
    {
        #region GameSystem

        // Debug
        public static Action IsTestingChanged;

        // Currency
        public static Action SaveCoinTotal;

        public static Action CoinTotalChanged;

        // Level Spawn
        public static Action CurrentLevelChanged;

        // Setting
        public static Action MusicChanged;
        public static Action SoundChanged;

        public static Action VibrationChanged;

        // Ads
        public static Action RequestBanner;
        public static Action ShowBanner;
        public static Action RequestInterstitial;
        public static Action ShowInterstitial;
        public static Action RequestReward;

        public static Action ShowReward;

        // Other
        public static Action CoinMove;
        public static Action ClickButton;
        public static Action<string> TrackClickButton;
        public static Action PurchaseFail;
        public static Action PurchaseSucceed;
        public static Action ClaimReward;

        #endregion
    }
}