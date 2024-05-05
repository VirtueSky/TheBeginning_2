using Base.Game;
using Base.UI;
using UnityEngine;
using VirtueSky.Audio;

namespace Base.UI
{
    public class PopupHome : UIPopup
    {
        [SerializeField] private SoundData musicHome;

        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            AudioManager.Instance.PlayMusic(musicHome);
        }

        public void OnClickStartGame()
        {
            GameManager.Instance.PlayCurrentLevel();
        }
    }
}