using System.Collections.Generic;
using Base.Data;
using Base.Global;
using UnityEngine;
using VirtueSky.Core;
using Virtuesky.Events;
using VirtueSky.Inspector;

public class ShowObject : MonoBehaviour
{
    public bool IsShowByTesting;
    public bool IsShowByLevel;
    public float DelayShowTime;

    [ShowIf(nameof(IsShowByLevel))] public List<int> LevelsShow;


    private bool IsLevelInLevelsShow()
    {
        foreach (int level in LevelsShow)
        {
            if (UserData.CurrentLevel == level)
            {
                return true;
            }
        }

        return false;
    }

    private bool EnableToShow()
    {
        bool testingCondition = !IsShowByTesting || (IsShowByTesting && UserData.IsTestingDebug);
        bool levelCondition = !IsShowByLevel || (IsShowByLevel && IsLevelInLevelsShow());
        return testingCondition && levelCondition;
    }

    private void Awake()
    {
        EventName.IsTestingChanged.AddListener(SetupByIsTesting);
        EventName.CurrentLevelChanged.AddListener(SetupByIndexLevel);
    }

    private void OnDestroy()
    {
        EventName.IsTestingChanged.RemoveListener(SetupByIsTesting);
        EventName.CurrentLevelChanged.RemoveListener(SetupByIndexLevel);
    }

    public void OnEnable()
    {
        Setup();
    }

    void SetupByIsTesting()
    {
        Setup();
    }

    void SetupByIndexLevel()
    {
        Setup();
    }


    public void Setup()
    {
        if (DelayShowTime > 0) gameObject.SetActive(false);
        App.Delay(DelayShowTime, () => { gameObject.SetActive(EnableToShow()); });
    }
}