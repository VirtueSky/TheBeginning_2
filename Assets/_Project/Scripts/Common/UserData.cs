using System;
using UnityEngine;
using VirtueSky.DataStorage;

public partial struct UserData
{
    #region GAME_DATA

    public static int PercentWinGift
    {
        get => Get(Constant.PERCENT_WIN_GIFT, 0);
        set => Set(Constant.PERCENT_WIN_GIFT, value);
    }

    public static bool IsFirstOpenGame
    {
        get => Get(Constant.IS_FIRST_OPEN_GAME, 0) == 1;
        set => Set(Constant.IS_FIRST_OPEN_GAME, value ? 1 : 0);
    }

    public static bool IsTesting
    {
        get => Get(Constant.IS_TESTING, 0) == 1;
        set
        {
            Set(Constant.IS_TESTING, value ? 1 : 0);
            Observer.DebugChanged?.Invoke();
        }
    }

    public static bool IsOffInterAds
    {
        get => Get(Constant.IS_OFF_INTER_ADS, false);
        set => Set(Constant.IS_OFF_INTER_ADS, value);
    }

    public static int CurrentLevel
    {
        get => Get(Constant.INDEX_LEVEL_CURRENT, 1);

        set
        {
            Set(Constant.INDEX_LEVEL_CURRENT, value >= 1 ? value : 1);
            Observer.CurrentLevelChanged?.Invoke();
        }
    }

    public static int GetNumberShowGameObject(string gameObjectID)
    {
        return Get($"{Constant.GAMEOBJECT_SHOW}_{gameObjectID}", 0);
    }

    public static void IncreaseNumberShowGameObject(string gameObjectID)
    {
        int value = GetNumberShowGameObject(gameObjectID);
        Set($"{Constant.GAMEOBJECT_SHOW}_{gameObjectID}", ++value);
    }

    public static int CurrencyTotal
    {
        get => Get(Constant.CURRENCY_TOTAL, 0);
        set
        {
            Observer.SaveCurrencyTotal?.Invoke();
            Set(Constant.CURRENCY_TOTAL, value);
            Observer.CurrencyTotalChanged?.Invoke();
        }
    }

    public static int ProgressAmount
    {
        get => Get(Constant.PROGRESS_AMOUNT, 0);
        set => Set(Constant.PROGRESS_AMOUNT, value);
    }

    public static bool IsItemEquipped(string itemIdentity)
    {
        return Get($"{Constant.EQUIP_ITEM}_{IdItemUnlocked}", false);
    }

    public static void SetItemEquipped(string itemIdentity, bool isEquipped = true)
    {
        Set($"{Constant.EQUIP_ITEM}_{IdItemUnlocked}", isEquipped);
    }

    public static string IdItemUnlocked = "";

    public static bool IsItemUnlocked
    {
        get => Get($"{Constant.UNLOCK_ITEM}_{IdItemUnlocked}", false);
        set => Set($"{Constant.UNLOCK_ITEM}_{IdItemUnlocked}", value);
    }

    #endregion

    #region SETTING_DATA

    public static bool BgSoundState
    {
        get => Get(Constant.BACKGROUND_SOUND_STATE, true);
        set
        {
            Set(Constant.BACKGROUND_SOUND_STATE, value);
            Observer.MusicChanged?.Invoke();
        }
    }

    public static bool FxSoundState
    {
        get => Get(Constant.FX_SOUND_STATE, true);
        set
        {
            Set(Constant.FX_SOUND_STATE, value);
            Observer.SoundChanged?.Invoke();
        }
    }

    public static bool VibrateState
    {
        get => Get(Constant.VIBRATE_STATE, true);
        set => Set(Constant.VIBRATE_STATE, value);
    }

    #endregion

    #region DAILY_REWARD

    public static bool IsClaimedTodayDailyReward()
    {
        return (int)(DateTime.Now - DateTime.Parse(LastDailyRewardClaimed)).TotalDays == 0;
    }

    public static bool IsStartLoopingDailyReward
    {
        get => PlayerPrefs.GetInt(Constant.IS_START_LOOPING_DAILY_REWARD, 0) == 1;
        set => PlayerPrefs.SetInt(Constant.IS_START_LOOPING_DAILY_REWARD, value ? 1 : 0);
    }

    public static string DateTimeStart
    {
        get => Get(Constant.DATE_TIME_START, DateTime.Now.ToString());
        set => Set(Constant.DATE_TIME_START, value);
    }

    public static int TotalPlayedDays =>
        (int)(DateTime.Now - DateTime.Parse(DateTimeStart)).TotalDays + 1;

    public static int DailyRewardDayIndex
    {
        get => Get(Constant.DAILY_REWARD_DAY_INDEX, 1);
        set => Set(Constant.DAILY_REWARD_DAY_INDEX, value);
    }

    public static string LastDailyRewardClaimed
    {
        get => Get(Constant.LAST_DAILY_REWARD_CLAIM, DateTime.Now.AddDays(-1).ToString());
        set => Set(Constant.LAST_DAILY_REWARD_CLAIM, value);
    }

    public static int TotalClaimDailyReward
    {
        get => Get(Constant.TOTAL_CLAIM_DAILY_REWARD, 0);
        set => Set(Constant.TOTAL_CLAIM_DAILY_REWARD, value);
    }

    #endregion

    #region FIREBASE

    // TOGGLE LEVEL AB TESTING? 0:NO, 1:YES
    public static int DEFAULT_USE_LEVEL_AB_TESTING = 0;

    public static int UseLevelABTesting
    {
        get => Get(Constant.USE_LEVEL_AB_TESTING, DEFAULT_USE_LEVEL_AB_TESTING);
        set => Get(Constant.USE_LEVEL_AB_TESTING, value);
    }

    // SET LEVEL TO ENABLE INTERSTITIAL
    public static int DEFAULT_LEVEL_TURN_ON_INTERSTITIAL = 5;

    public static int LevelTurnOnInterstitial
    {
        get => Get(Constant.LEVEL_TURN_ON_INTERSTITIAL,
            DEFAULT_LEVEL_TURN_ON_INTERSTITIAL);
        set => Set(Constant.LEVEL_TURN_ON_INTERSTITIAL, value);
    }

    // SET COUNTER VARIABLE
    public static int DEFAULT_COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL = 2;

    public static int CounterNumbBetweenTwoInterstitial
    {
        get => Get(Constant.COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL,
            DEFAULT_COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL);
        set => Set(Constant.COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL, value);
    }

    // SET TIME TO ENABLE BETWEEN 2 INTERSTITIAL (ON WIN,LOSE,REPLAY GAME)
    public static int DEFAULT_SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL = 30;

    public static int TimeWinBetweenTwoInterstitial
    {
        get => Get(Constant.SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL,
            DEFAULT_SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL);
        set => Set(Constant.SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL, value);
    }

    // TOGGLE SHOW INTERSTITIAL ON LOSE GAME ? 0:NO, 1:YES
    public static int DEFAULT_SHOW_INTERSTITIAL_ON_LOSE_GAME = 0;

    public static int UseShowInterstitialOnLoseGame
    {
        get => Get(Constant.SHOW_INTERSTITIAL_ON_LOSE_GAME, DEFAULT_SHOW_INTERSTITIAL_ON_LOSE_GAME);
        set => Set(Constant.SHOW_INTERSTITIAL_ON_LOSE_GAME, value);
    }

    // SET TIME TO ENABLE BETWEEN 2 INTERSTITIAL (ON LOSE GAME)
    public static int DEFAULT_SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL = 45;

    public static int TimeLoseBetweenTwoInterstitial
    {
        get => Get(Constant.SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL,
            DEFAULT_SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL);
        set => Set(Constant.SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL, value);
    }

    public static int GetLogEventFirstStartLevel(int level)
    {
        return Get($"First_Start_Level_{level}", 0);
    }

    public static void SetLogEventFirstStartLevel(int level, bool isOwned = true)
    {
        Set($"First_Start_Level_{level}", isOwned ? 1 : 0);
    }

    #endregion

    public static T Get<T>(string key, T defaultValue = default) => GameData.Get(key, defaultValue);
    public static void Set<T>(string key, T data) => GameData.Set(key, data);
}