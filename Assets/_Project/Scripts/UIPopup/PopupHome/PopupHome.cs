using Base.Game;
using UnityEngine;
using VirtueSky.Audio;
using VirtueSky.Pattern;

namespace Base.UI
{
    public class PopupHome : UIPopup
    {
        [SerializeField] private SoundData musicHome;

        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            musicHome.PlayMusic();
        }

        protected override void OnAfterShow()
        {
            base.OnAfterShow();
            EventName.PopupHomeShowed.Raise();
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