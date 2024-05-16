using System;
using System.Collections.Generic;
using System.Linq;
using Base.Data;
using Base.Global;
using Base.Services;
using UnityEngine;
using VirtueSky.Inspector;

namespace Base.UI
{
    public class PopupDailyReward : UIPopup
    {
        [TitleColor("Attribute", CustomColor.Lavender, CustomColor.Cornsilk)]
        public GameObject btnWatchVideo;

        public GameObject btnClaim;
        [ReadOnly] public DailyRewardItem currentItem;
        public List<DailyRewardItem> DailyRewardItems => GetComponentsInChildren<DailyRewardItem>().ToList();

        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            ResetDailyReward();
            Setup();
        }


        public void ResetDailyReward()
        {
            if (!UserData.IsClaimedTodayDailyReward() && UserData.DailyRewardDayIndex == 29)
            {
                UserData.DailyRewardDayIndex = 1;
                UserData.IsStartLoopingDailyReward = true;
            }
        }

        private bool IsCurrentItem(int index)
        {
            return UserData.DailyRewardDayIndex == index;
        }

        public void Setup()
        {
            SetUpItems();
        }

        private void SetUpItems()
        {
            var week = (UserData.DailyRewardDayIndex - 1) / 7;
            if (UserData.IsClaimedTodayDailyReward()) week = (UserData.DailyRewardDayIndex - 2) / 7;

            for (var i = 0; i < 7; i++)
            {
                var item = DailyRewardItems[i];
                item.SetUp(i + 7 * week);
                if (IsCurrentItem(item.dayIndex)) currentItem = item;
            }

            btnWatchVideo.SetActive(false);
            btnClaim.SetActive(false);
            if (currentItem && currentItem.DailyRewardItemState == DailyRewardItemState.ReadyToClaim)
            {
                btnWatchVideo.SetActive(currentItem.DailyRewardData.dailyRewardType == DailyRewardType.Coin);
                btnClaim.SetActive(true);
            }
        }

        public void OnClickBtnClaimX5Video()
        {
            AdsManager.Instance.ShowRewardAds(() =>
            {
                currentItem.OnClaim(true, () =>
                {
                    Observer.OnClaimDailyReward?.Invoke();
                    Setup();
                });
            });
        }

        public void OnClickBtnClaim()
        {
            currentItem.OnClaim(false, () =>
            {
                Observer.OnClaimDailyReward?.Invoke();
                Setup();
            });
        }

        public void OnClickNextDay()
        {
            UserData.LastDailyRewardClaimed = DateTime.Now.AddDays(-1).ToString();
            ResetDailyReward();
            Setup();
        }
    }
}