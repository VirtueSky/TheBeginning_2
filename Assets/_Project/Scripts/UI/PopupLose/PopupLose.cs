using System.Reflection;

public class PopupLose : Popup
{
    protected override void BeforeShow()
    {
        base.BeforeShow();
        PopupController.Instance.Show<PopupUI>();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickReplay()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        GameManager.Instance.ReplayGame();
    }
}
