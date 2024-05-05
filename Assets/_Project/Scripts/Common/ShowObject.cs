using System.Collections.Generic;
using Base.Data;
using Base.Global;
using UnityEngine;
using VirtueSky.Core;
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
        bool testingCondition = !IsShowByTesting || (IsShowByTesting && UserData.IsTesting);
        bool levelCondition = !IsShowByLevel || (IsShowByLevel && IsLevelInLevelsShow());
        return testingCondition && levelCondition;
    }

    private void Awake()
    {
        Observer.IsTestingChanged += SetupByIsTesting;
        Observer.CurrentLevelChanged += SetupByIndexLevel;
    }

    private void OnDestroy()
    {
        Observer.IsTestingChanged -= SetupByIsTesting;
        Observer.CurrentLevelChanged -= SetupByIndexLevel;
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