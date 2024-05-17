using System;
using Base.Global;
using UnityEngine;
using VirtueSky.DataStorage;

namespace Base.Data
{
    public partial struct UserData
    {
        #region GAME_DATA

        public static int PercentWinGift
        {
            get => GameData.Get(Constant.PERCENT_WIN_GIFT, 0);
            set => GameData.Set(Constant.PERCENT_WIN_GIFT, value);
        }

        public static bool IsFirstOpenGame
        {
            get => GameData.Get(Constant.IS_FIRST_OPEN_GAME, false);
            set => GameData.Set(Constant.IS_FIRST_OPEN_GAME, value);
        }


        public static int CurrentLevel
        {
            get => GameData.Get(Constant.INDEX_LEVEL_CURRENT, 1);

            set
            {
                GameData.Set(Constant.INDEX_LEVEL_CURRENT, value >= 1 ? value : 1);
                GameData.Save();
                Observer.CurrentLevelChanged?.Invoke();
            }
        }

        public static int GetNumberShowGameObject(string gameObjectID)
        {
            return GameData.Get($"{Constant.GAMEOBJECT_SHOW}_{gameObjectID}", 0);
        }

        public static void IncreaseNumberShowGameObject(string gameObjectID)
        {
            int value = GetNumberShowGameObject(gameObjectID);
            GameData.Set($"{Constant.GAMEOBJECT_SHOW}_{gameObjectID}", ++value);
        }

        public static int CoinTotal
        {
            get => GameData.Get(Constant.CURRENCY_TOTAL, 0);
            set
            {
                Observer.SaveCoinTotal?.Invoke();
                GameData.Set(Constant.CURRENCY_TOTAL, value);
                GameData.Save();
                Observer.CoinTotalChanged?.Invoke();
            }
        }

        public static int ProgressAmount
        {
            get => GameData.Get(Constant.PROGRESS_AMOUNT, 0);
            set => GameData.Set(Constant.PROGRESS_AMOUNT, value);
        }

        public static bool IsItemEquipped(string itemIdentity)
        {
            return GameData.Get($"{Constant.EQUIP_ITEM}_{IdItemUnlocked}", false);
        }

        public static void SetItemEquipped(string itemIdentity, bool isEquipped = true)
        {
            GameData.Set($"{Constant.EQUIP_ITEM}_{IdItemUnlocked}", isEquipped);
        }

        public static string IdItemUnlocked = "";

        public static bool IsItemUnlocked
        {
            get => GameData.Get($"{Constant.UNLOCK_ITEM}_{IdItemUnlocked}", false);
            set => GameData.Set($"{Constant.UNLOCK_ITEM}_{IdItemUnlocked}", value);
        }

        #endregion


        #region DAILY_REWARD

        public static bool IsClaimedTodayDailyReward()
        {
            return (int)(DateTime.Now - DateTime.Parse(LastDailyRewardClaimed)).TotalDays == 0;
        }

        public static bool IsStartLoopingDailyReward
        {
            get => GameData.Get(Constant.IS_START_LOOPING_DAILY_REWARD, 0) == 1;
            set => GameData.Set(Constant.IS_START_LOOPING_DAILY_REWARD, value ? 1 : 0);
        }

        public static string DateTimeStart
        {
            get => GameData.Get(Constant.DATE_TIME_START, DateTime.Now.ToString());
            set => GameData.Set(Constant.DATE_TIME_START, value);
        }

        public static int TotalPlayedDays =>
            (int)(DateTime.Now - DateTime.Parse(DateTimeStart)).TotalDays + 1;

        public static int DailyRewardDayIndex
        {
            get => GameData.Get(Constant.DAILY_REWARD_DAY_INDEX, 1);
            set => GameData.Set(Constant.DAILY_REWARD_DAY_INDEX, value);
        }

        public static string LastDailyRewardClaimed
        {
            get => GameData.Get(Constant.LAST_DAILY_REWARD_CLAIM, DateTime.Now.AddDays(-1).ToString());
            set => GameData.Set(Constant.LAST_DAILY_REWARD_CLAIM, value);
        }

        public static int TotalClaimDailyReward
        {
            get => GameData.Get(Constant.TOTAL_CLAIM_DAILY_REWARD, 0);
            set => GameData.Set(Constant.TOTAL_CLAIM_DAILY_REWARD, value);
        }

        #endregion

        public static int AdsCounter
        {
            get => GameData.Get(Constant.ADS_COUNTER, 0);
            set => GameData.Set(Constant.ADS_COUNTER, value);
        }
    }
}