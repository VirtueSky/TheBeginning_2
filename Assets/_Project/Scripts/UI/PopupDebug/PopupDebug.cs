using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDebug : Popup
{
    public TMP_InputField SetLevel;
    public TMP_InputField SetCoin;
    public Toggle ToggleTesting;
    public Toggle ToggleOffInterAds;

    public void OnEnable()
    {
        ToggleTesting.isOn = Data.IsTesting;
        ToggleOffInterAds.isOn = Data.IsOffInterAds;
    }

    public void OnClickAccept()
    {
        if (SetLevel.text != null && SetLevel.text != "")
        {
            Data.CurrentLevel = int.Parse(SetLevel.text);
            GameManager.Instance.PrepareLevel();
            GameManager.Instance.StartGame();
        }

        if (SetCoin.text != null && SetCoin.text != "")
        {
            Data.CurrencyTotal = int.Parse(SetCoin.text);
        }

        SetCoin.text = string.Empty;
        SetLevel.text = string.Empty;
        gameObject.SetActive(false);
    }

    public void ChangeTestingState()
    {
        Data.IsTesting = ToggleTesting.isOn;
    }

    public void OnClickOffInterAds()
    {
        Data.IsOffInterAds = ToggleOffInterAds.isOn;
    }

    public void OnClickFPSBtn()
    {
        GameManager.Instance.ChangeAFpsState();
    }

    public void OnClickUnLockAllSKin()
    {
        ConfigController.ItemConfig.UnlockAllSkin();
    }
}