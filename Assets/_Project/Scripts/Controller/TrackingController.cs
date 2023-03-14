using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Firebase.Analytics;
using UnityEngine;

public class TrackingController : SingletonDontDestroy<TrackingController>
{
    void Start()
    {
        Observer.StartLevel += OnStartLevel;
        Observer.WinLevel += OnWinLevel;
        Observer.LoseLevel += OnLoseLevel;
        Observer.FirstStartLevel += OnFirstStartLevel;

        Observer.TrackClickButton += OnClickButton;
        Observer.RequestBanner += OnRequestBanner;
        Observer.ShowBanner += OnShowBanner;
        Observer.RequestInterstitial += OnRequestInterstitial;
        Observer.ShowInterstitial += OnShowInterstitial;
        Observer.RequestReward += OnRequestReward;
        Observer.ShowReward += OnShowReward;
    }

    #region TrackingGameplay

    private void OnStartLevel(Level level)
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Parameter[] parameters =
        {
            new Parameter("level_name", level.gameObject.name),
        };
        LogEvent(function.Name, parameters);
    }

    private void OnFirstStartLevel(Level level)
    {
        if (Data.GetLogEventFirstStartLevel(Data.CurrentLevel) != 0) return;
        try
        {
            Data.SetLogEventFirstStartLevel(Data.CurrentLevel);
            MethodBase function = MethodBase.GetCurrentMethod();
            Parameter[] parameters =
            {
                new Parameter("First_Start_Level", level.gameObject.name)
            };
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR while LogEventStartLevel: " + e.ToString());
            throw;
        }
    }

    private void OnLoseLevel(Level level)
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Parameter[] parameters =
        {
            new Parameter("level_name", level.gameObject.name),
        };
        LogEvent(function.Name, parameters);
    }

    private void OnWinLevel(Level level)
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Parameter[] parameters =
        {
            new Parameter("level_name", level.gameObject.name),
        };
        LogEvent(function.Name, parameters);
    }

    private void OnReplayLevel(Level level)
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        Parameter[] parameters =
        {
            new Parameter("level_name", level.gameObject.name),
        };
        LogEvent(function.Name, parameters);
    }

    #endregion

    #region TrackingGameSystem

    private void OnClickButton(string buttonName)
    {
        LogEvent(buttonName);
    }

    public static void OnRequestInterstitial()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        LogEvent(function.Name);
    }

    public static void OnShowInterstitial()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        LogEvent(function.Name);
    }

    public static void OnRequestReward()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        LogEvent(function.Name);
    }

    public static void OnShowReward()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        LogEvent(function.Name);
    }

    public static void OnRequestBanner()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        LogEvent(function.Name);
    }

    public static void OnShowBanner()
    {
        MethodBase function = MethodBase.GetCurrentMethod();
        LogEvent(function.Name);
    }

    #endregion

    #region BaseLogFunction

    public static bool IsMobile()
    {
        return (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer);
    }

    public static void LogEvent(string paramName, Parameter[] parameters)
    {
        if (!IsMobile()) return;
        try
        {
            FirebaseAnalytics.LogEvent(paramName, parameters);
        }
        catch (Exception e)
        {
            Debug.LogError("Event log error: " + e.ToString());
            throw;
        }
    }

    public static void LogEvent(string paramName)
    {
        if (!IsMobile()) return;
        try
        {
            FirebaseAnalytics.LogEvent(paramName);
        }
        catch (Exception e)
        {
            Debug.LogError("Event log error: " + e.ToString());
            throw;
        }
    }

    #endregion
}