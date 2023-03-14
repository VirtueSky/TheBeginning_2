using System.Reflection;

public class PopupHome : Popup
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

    public void OnClickStart()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        GameManager.Instance.StartGame();
    }

    public void OnClickDebug()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        PopupController.Instance.Show<PopupDebug>();
    }

    public void OnClickSetting()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        PopupController.Instance.Show<PopupSetting>();
    }

    public void OnClickDailyReward()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        PopupController.Instance.Show<PopupDailyReward>();
    }

    public void OnClickShop()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        PopupController.Instance.Show<PopupShop>();
    }
    
    public void OnClickTest()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);
        
        PopupController.Instance.Show<PopupTest>();
    }
}
