using Base.Data;
using Base.Game;
using Base.Global;
using TMPro;
using UnityEngine;
using VirtueSky.Audio;
using Virtuesky.Events;


namespace Base.UI
{
    public class GameplayPopup : UIPopup
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI levelTypeText;
        [SerializeField] private SoundData musicInGame;


        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            Setup();
            EventName.CurrentLevelChanged.AddListener(Setup);
            musicInGame.PlayMusic();
        }

        protected override void OnBeforeHide()
        {
            base.OnBeforeHide();
            EventName.CurrentLevelChanged.RemoveListener(Setup);
        }

        public void Setup()
        {
            levelText.text = $"Level {UserData.CurrentLevel}";
            // LevelTypeText.text = $"Level ";
        }

        public void OnClickHome()
        {
            GameManager.Instance.BackHome();
        }

        public void OnClickReplay()
        {
            //AppControlAds.ShowInterstitial(() => { replayEvent.Raise(); });
        }

        public void OnClickPrevious()
        {
            GameManager.Instance.BackLevel();
        }

        public void OnClickSkip()
        {
            GameManager.Instance.NextLevel();
        }

        public void OnClickLose()
        {
            GameManager.Instance.LoseLevel(1);
        }

        public void OnClickWin()
        {
            GameManager.Instance.WinLevel(1);
        }
    }
}