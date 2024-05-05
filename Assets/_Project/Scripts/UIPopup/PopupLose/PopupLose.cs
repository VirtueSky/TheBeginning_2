using Base.Game;

namespace Base.UI
{
    public class PopupLose : UIPopup
    {
        public void OnClickReplay()
        {
            Hide();
            GameManager.Instance.ReplayLevel();
        }
    }
}