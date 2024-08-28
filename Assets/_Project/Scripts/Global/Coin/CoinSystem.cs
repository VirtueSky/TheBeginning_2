using System;
using UnityEngine;
using VirtueSky.DataStorage;
using VirtueSky.Inspector;

[EditorIcon("icon_controller"), HideMonoScript]
public class CoinSystem : MonoBehaviour
{
    public static event Action OnAddCoinCompletedEvent;
    public static event Action OnMinusCoinCompletedEvent;
    public static event Action<Vector3> OnSetFromCoinGenerateEvent;
    private static event Action<int, Vector3> OnAddCoinEvent;
    private static event Action<int> OnMinusCoinEvent;
    private static event Action<int, Vector3> OnSetCoinEvent;

    private void Awake()
    {
        OnAddCoinEvent += InternalAddCoin;
        OnMinusCoinEvent += InternalMinusCoin;
        OnSetCoinEvent += InternalSetCoin;
    }

    private void OnDestroy()
    {
        OnAddCoinEvent -= InternalAddCoin;
        OnMinusCoinEvent -= InternalMinusCoin;
        OnSetCoinEvent -= InternalSetCoin;
    }

    private static int CurrentCoin
    {
        get => GameData.Get(Constant.CURRENT_COIN, 0);
        set
        {
            GameData.Set(Constant.CURRENT_COIN, value);
            GameData.Save();
        }
    }

    private void InternalSetCoin(int value, Vector3 posGenerateCoin = default)
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

    private void InternalAddCoin(int value, Vector3 posGenerateCoin = default)
    {
        CurrentCoin += value;
        OnAddCoinCompletedEvent?.Invoke();
        if (posGenerateCoin != default)
        {
            OnSetFromCoinGenerateEvent?.Invoke(posGenerateCoin);
        }
    }

    private void InternalMinusCoin(int value)
    {
        CurrentCoin -= value;
        OnMinusCoinCompletedEvent?.Invoke();
    }

    #region Api

    public static void AddCoin(int value, Vector3 posGenerateCoin = default) =>
        OnAddCoinEvent?.Invoke(value, posGenerateCoin);

    public static void MinusCoin(int value) => OnMinusCoinEvent?.Invoke(value);

    public static void SetCoin(int value, Vector3 posGenerateCoin = default) =>
        OnSetCoinEvent?.Invoke(value, posGenerateCoin);

    public static int GetCurrentCoin() => CurrentCoin;

    #endregion
}