using System;
using Base.Game;
using Base.Services;
using Base.UI;
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
            AudioManager.Instance.PlayMusic(musicHome);
        }

        public void OnClickStartGame()
        {
            GameManager.Instance.PlayCurrentLevel();
        }

        public void OnClickSettings()
        {
            PopupManager.Instance.Show<PopupSetting>(false);
        }

        public void OnClickDailyReward()
        {
            PopupManager.Instance.Show<PopupDailyReward>(false);
        }
    }
}