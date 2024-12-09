using Base.Game;

namespace Base.UI
{
    public class LosePopup : UIPopup
    {
        public void OnClickReplay()
        {
            Hide();
            GameManager.Instance.ReplayLevel();
        }
    }
}