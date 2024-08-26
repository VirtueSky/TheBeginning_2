using System;
using UnityEngine;
using VirtueSky.DataStorage;

public class CoinSystem
{
    public static event Action OnAddCoinCompletedEvent;
    public static event Action OnMinusCoinCompletedEvent;
    public static event Action<Vector3> OnSetFromCoinGenerateEvent;

    private static int CurrentCoin
    {
        get => GameData.Get(Constant.CURRENT_COIN, 0);
        set
        {
            GameData.Set(Constant.CURRENT_COIN, value);
            GameData.Save();
        }
    }

    public static void SetCoin(int value, Vector3 posGenerateCoin = default)
    {
        if (value > CurrentCoin)
        {
            CurrentCoin = value;
            OnAddCoinCompletedEvent?.Invoke();
            if (posGenerateCoin != default)
            {
                OnSetFromCoinGenerateEvent?.Invoke(posGenerateCoin);
            }
        }
        else if (value < CurrentCoin)
        {
            CurrentCoin = value;
            OnMinusCoinCompletedEvent?.Invoke();
        }
    }

    public static void AddCoin(int value, Vector3 posGenerateCoin = default)
    {
        CurrentCoin += value;
        OnAddCoinCompletedEvent?.Invoke();
        if (posGenerateCoin != default)
        {
            OnSetFromCoinGenerateEvent?.Invoke(posGenerateCoin);
        }
    }

    public static void MinusCoin(int value)
    {
        CurrentCoin -= value;
        OnMinusCoinCompletedEvent?.Invoke();
    }

    public static int GetCurrentCoin() => CurrentCoin;
}