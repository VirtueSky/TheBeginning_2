using System;
using System.Reflection;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.Serialization;

public class FirebaseController : SingletonDontDestroy<FirebaseController>
{
    public DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public bool isInitialized;

    protected override void Awake()
    {
        Initialize();
    }

    void Start()
    {
        Observer.StartLevel += OnStartLevel;
        Observer.WinLevel += OnWinLevel;
        Observer.LoseLevel += OnLoseLevel;

        Observer.TrackClickButton += OnClickButton;
        Observer.RequestBanner += OnRequestBanner;
        Observer.ShowBanner += OnShowBanner;
        Observer.RequestInterstitial += OnRequestInterstitial;
        Observer.ShowInterstitial += OnShowInterstitial;
        Observer.RequestReward += OnRequestReward;
        Observer.ShowReward += OnShowReward;
    }

    #region FirebaseInitGetRemoteConfig

    public void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();

                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private async void InitializeFirebase()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        var defaults = new System.Collections.Generic.Dictionary<string, object>
        {
            { Constant.USE_LEVEL_AB_TESTING, UserData.DEFAULT_USE_LEVEL_AB_TESTING },
            { Constant.LEVEL_TURN_ON_INTERSTITIAL, UserData.DEFAULT_LEVEL_TURN_ON_INTERSTITIAL },
            {
                Constant.COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL,
                UserData.DEFAULT_COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL
            },
            {
                Constant.SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL,
                UserData.DEFAULT_SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL
            },
            { Constant.SHOW_INTERSTITIAL_ON_LOSE_GAME, UserData.DEFAULT_SHOW_INTERSTITIAL_ON_LOSE_GAME },
            {
                Constant.SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL,
                UserData.DEFAULT_SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL
            },
        };
        await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
            .ContinueWithOnMainThread(task =>
            {
                // [END set_defaults]
                Debug.Log("RemoteConfig configured and ready!");
            });

        await FetchDataAsync();
    }

    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask =
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }

        return fetchTask.ContinueWithOnMainThread(tast =>
        {
            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            //SET NEW DATA FROM REMOTE CONFIG
            if (info.LastFetchStatus == LastFetchStatus.Success)
            {
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                            info.FetchTime));
                    });

                UserData.UseLevelABTesting = int.Parse(FirebaseRemoteConfig.DefaultInstance
                    .GetValue(Constant.USE_LEVEL_AB_TESTING).StringValue);
                UserData.LevelTurnOnInterstitial = int.Parse(FirebaseRemoteConfig.DefaultInstance
                    .GetValue(Constant.LEVEL_TURN_ON_INTERSTITIAL).StringValue);
                UserData.CounterNumbBetweenTwoInterstitial = int.Parse(FirebaseRemoteConfig.DefaultInstance
                    .GetValue(Constant.COUNTER_NUMBER_BETWEEN_TWO_INTERSTITIAL).StringValue);
                UserData.TimeWinBetweenTwoInterstitial = int.Parse(FirebaseRemoteConfig.DefaultInstance
                    .GetValue(Constant.SPACE_TIME_WIN_BETWEEN_TWO_INTERSTITIAL).StringValue);
                UserData.UseShowInterstitialOnLoseGame = int.Parse(FirebaseRemoteConfig.DefaultInstance
                    .GetValue(Constant.SHOW_INTERSTITIAL_ON_LOSE_GAME).StringValue);
                UserData.TimeLoseBetweenTwoInterstitial = int.Parse(FirebaseRemoteConfig.DefaultInstance
                    .GetValue(Constant.SPACE_TIME_LOSE_BETWEEN_TWO_INTERSTITIAL).StringValue);

                Debug.Log("<color=Green>Firebase Remote Config Fetching Values</color>");
                Debug.Log($"<color=Green>Data.UseLevelABTesting: {UserData.UseLevelABTesting}</color>");
                Debug.Log($"<color=Green>Data.LevelTurnOnInterstitial: {UserData.LevelTurnOnInterstitial}</color>");
                Debug.Log(
                    $"<color=Green>Data.CounterNumbBetweenTwoInterstitial: {UserData.CounterNumbBetweenTwoInterstitial}</color>");
                Debug.Log(
                    $"<color=Green>Data.TimeWinBetweenTwoInterstitial: {UserData.TimeWinBetweenTwoInterstitial}</color>");
                Debug.Log(
                    $"<color=Green>Data.UseShowInterstitialOnLoseGame: {UserData.UseShowInterstitialOnLoseGame}</color>");
                Debug.Log(
                    $"<color=Green>Data.TimeLoseBetweenTwoInterstitial: {UserData.TimeLoseBetweenTwoInterstitial}</color>");
                Debug.Log("<color=Green>Firebase Remote Config Fetching completed!</color>");
            }
            else
            {
                Debug.Log("Fetching data did not completed!");
            }

            isInitialized = true;
        });
    }

    #endregion

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