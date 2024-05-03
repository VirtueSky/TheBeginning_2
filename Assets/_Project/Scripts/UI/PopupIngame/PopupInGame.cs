using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class PopupInGame : Popup
{
    [Header("Components")] public TextMeshProUGUI LevelText;
    public TextMeshProUGUI LevelTypeText;

    private List<UIEffect> UIEffects => GetComponentsInChildren<UIEffect>().ToList();

    public void Start()
    {
        Observer.WinLevel += HideUI;
        Observer.LoseLevel += HideUI;
    }

    public void OnDestroy()
    {
        Observer.WinLevel -= HideUI;
        Observer.LoseLevel -= HideUI;
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        //if (!Data.IsTesting) AdsManager.ShowBanner();
        Setup();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        //AdsManager.HideBanner();
    }

    public void Setup()
    {
        LevelText.text = $"Level {UserData.CurrentLevel}";
        LevelTypeText.text = $"Level {(UserData.UseLevelABTesting == 0 ? "A" : "B")}";
    }

    public void OnClickHome()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Observer.TrackClickButton?.Invoke(function.Name);

        GameManager.Instance.ReturnHome();
    }

    public void OnClickReplay()
    {
        if (UserData.IsTesting)
        {
            GameManager.Instance.ReplayGame();
        }
        else
        {
            // AdsManager.ShowInterstitial(() =>
            // {
            //    MethodBase function = MethodBase.GetCurrentMethod();
            //    Observer.TrackClickButton?.Invoke(function.Name);
            //    
            //    GameManager.Instance.ReplayGame();
            // });
        }
    }

    public void OnClickPrevious()
    {
        GameManager.Instance.BackLevel();
    }

    public void OnClickSkip()
    {
        if (UserData.IsTesting)
        {
            GameManager.Instance.NextLevel();
        }
        else
        {
            // AdsManager.ShowRewardAds(() =>
            // {
            //    MethodBase function = MethodBase.GetCurrentMethod();
            //    Observer.TrackClickButton?.Invoke(function.Name);
            //    
            //    GameManager.Instance.NextLevel();
            // });
        }
    }

    public void OnClickLevelA()
    {
        UserData.UseLevelABTesting = 0;
        GameManager.Instance.ReplayGame();
    }

    public void OnClickLevelB()
    {
        UserData.UseLevelABTesting = 1;
        GameManager.Instance.ReplayGame();
    }

    public void OnClickLose()
    {
        GameManager.Instance.OnLoseGame(1f);
    }

    public void OnClickWin()
    {
        GameManager.Instance.OnWinGame(1f);
    }

    private void HideUI(Level level = null)
    {
        foreach (UIEffect item in UIEffects)
        {
            item.PlayAnim();
        }
    }
}