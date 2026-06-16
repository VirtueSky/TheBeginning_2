using System;
using Base.Data;
using UnityEngine;
using VirtueSky.DataStorage;
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigGenerated;

namespace Base.Global.Currency
{
    /// <summary>
    /// Coin currency system.
    /// Kế thừa từ BaseCurrencySystem và giữ backward compatible API.
    /// </summary>
    [EditorIcon("icon_controller"), HideMonoScript]
    public class CoinSystem : BaseCurrencySystem<CoinSystem>
    {
        [SerializeField] private int initialCoinAmount = 0;
        protected override CurrencyType CurrencyId => CurrencyType.Coin;
        protected override string StorageKey => "CURRENT_COIN";
        protected override string DisplayName => "Coins";

        private const string InitializedKey = "COIN_INITIALIZED";

        private bool IsInitialized
        {
            get => GameData.Get(InitializedKey, false);
            set => GameData.Set(InitializedKey, value);
        }

        protected override void Awake()
        {
            base.Awake();
            // Initialization is now handled by CoinSystemInitializer after RemoteConfig loads
        }

        /// <summary>
        /// Initialize coin balance from remote config.
        /// Called by CoinSystemInitializer after RemoteConfig is loaded.
        /// </summary>
        public void InitializeFromRemoteConfig()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                InitFirstCoin();
            }
        }


        /// <summary>
        /// Legacy event kích hoạt khi coin được thêm.
        /// Giữ để backward compatibility.
        /// </summary>
        public static event Action OnAddCoinCompletedEvent;

        /// <summary>
        /// Legacy event kích hoạt khi coin bị trừ.
        /// Giữ để backward compatibility.
        /// </summary>
        public static event Action OnMinusCoinCompletedEvent;

        /// <summary>
        /// Legacy event kích hoạt khi coin được set với source position.
        /// Giữ để backward compatibility cho CoinGenerate.
        /// </summary>
        public static event Action<Vector3> OnSetFromCoinGenerateEvent;



        protected override void OnAdded(int amount, Vector3 position)
        {
            base.OnAdded(amount, position);

            // Trigger legacy events
            OnAddCoinCompletedEvent?.Invoke();

            if (position != default)
            {
                OnSetFromCoinGenerateEvent?.Invoke(position);
            }
        }

        protected override void OnSubtracted(int amount)
        {
            base.OnSubtracted(amount);

            // Trigger legacy event
            OnMinusCoinCompletedEvent?.Invoke();
        }



        /// <summary>
        /// Thêm coin với optional source position cho animation.
        /// Legacy static method - delegates to instance.
        /// </summary>
        public static void AddCoin(int value, Vector3 posGenerateCoin = default)
        {
            Instance.Add(value, posGenerateCoin);
        }

        /// <summary>
        /// Trừ coin.
        /// Legacy static method - delegates to instance.
        /// </summary>
        public static void MinusCoin(int value)
        {
            Instance.Subtract(value);
        }

        /// <summary>
        /// Đặt coin thành giá trị cụ thể với optional source position.
        /// Legacy static method - delegates to instance.
        /// </summary>
        public static void SetCoin(int value, Vector3 posGenerateCoin = default)
        {
            Instance.Set(value, posGenerateCoin);
        }

        /// <summary>
        /// Lấy số coin hiện tại.
        /// Legacy static method - delegates to instance.
        /// </summary>
        public static int GetCurrentCoin()
        {
            return Instance.Get();
        }

        private void InitFirstCoin()
        {
            
        }
    }
}
