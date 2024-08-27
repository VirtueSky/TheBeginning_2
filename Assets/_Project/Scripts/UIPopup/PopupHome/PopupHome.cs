using Base.Game;
using Base.Services;
using UnityEngine;
using VirtueSky.Audio;

namespace Base.UI
{
    public class PopupHome : UIPopup
    {
        [SerializeField] private SoundData musicHome;

        private void Start()
        {
            NotificationInGame.Instance.Show("Welcome!");
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
            PopupManager.Show<PopupSetting>(false);
        }

        public void OnClickDailyReward()
        {
            PopupManager.Show<PopupDailyReward>(false);
        }

        public void OnClickPopupLeaderboard()
        {
            PopupManager.Show<PopupLeaderboard>(false);
        }
    }
}