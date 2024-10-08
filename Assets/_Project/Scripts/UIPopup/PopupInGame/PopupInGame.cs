using Base.Data;
using Base.Game;
using Base.Global;
using TMPro;
using UnityEngine;
using VirtueSky.Audio;


namespace Base.UI
{
    public class PopupInGame : UIPopup
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI levelTypeText;
        [SerializeField] private SoundData musicInGame;


        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            Setup();
            Observer.CurrentLevelChanged += Setup;
            musicInGame.PlayMusic();
        }

        protected override void OnBeforeHide()
        {
            base.OnBeforeHide();
            Observer.CurrentLevelChanged -= Setup;
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