using Base.Game;

namespace Base.UI
{
    public class PopupWin : UIPopup
    {
        public void OnClickContinue()
        {
            GameManager.Instance.PlayCurrentLevel();
            Hide();
        }

        public void OnClickAds()
        {
        }
    }
}