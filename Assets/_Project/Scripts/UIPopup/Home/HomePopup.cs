using Base.Game;
using Base.Services;
using UnityEngine;
using VirtueSky.Audio;

namespace Base.UI
{
    public class HomePopup : UIPopup
    {
        [SerializeField] private SoundData musicHome;

        private void Start()
        {
        }

        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            musicHome.PlayMusic();
        }

        public void OnClickStartGame()
        {
            GameManager.Instance.PlayCurrentLevel();
        }

        public void OnClickSettings()
        {
            PopupManager.Show<SettingPopup>(false);
        }

        public void OnClickDailyReward()
        {
            PopupManager.Show<DailyRewardPopup>(false);
        }

        public void OnClickPopupLeaderboard()
        {
            PopupManager.Show<LeaderboardPopup>(false);
        }
    }
}