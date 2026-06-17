using System;
using UnityEngine;
using VirtueSky.DataStorage;
using VirtueSky.Pattern;

namespace Base.Global.Currency
{
    /// <summary>
    /// Abstract base class cho tất cả currency systems.
    /// Sử dụng CRTP pattern để mỗi currency type có singleton riêng.
    /// </summary>
    /// <typeparam name="T">Concrete currency system type</typeparam>
    public abstract class BaseCurrencySystem<T> : Singleton<T> where T : MonoBehaviour
    {

        protected override void Awake()
        {
            base.Awake();
            LoadBalance();
        }

        /// <summary>
        /// Unique identifier cho currency type (ví dụ: "coin", "diamond", "gem")
        /// </summary>
        protected abstract CurrencyType CurrencyId { get; }

        /// <summary>
        /// Storage key để lưu balance vào GameData
        /// </summary>
        protected abstract string StorageKey { get; }

        /// <summary>
        /// Tên hiển thị của currency (ví dụ: "Coins", "Diamonds")
        /// </summary>
        protected abstract string DisplayName { get; }



        /// <summary>
        /// Kích hoạt khi currency balance thay đổi.
        /// Parameters: (oldValue, newValue)
        /// </summary>
        public static event Action<int, int> OnCurrencyChanged;

        /// <summary>
        /// Kích hoạt khi currency được thêm với source position.
        /// Parameters: (amount, sourcePosition)
        /// </summary>
        public static event Action<int, Vector3> OnCurrencyAdded;

        /// <summary>
        /// Kích hoạt khi currency bị trừ.
        /// Parameters: (amount)
        /// </summary>
        public static event Action<int> OnCurrencySubtracted;



        private int _currentBalance;

        /// <summary>
        /// Balance hiện tại của currency.
        /// Tự động save khi set.
        /// </summary>
        protected int CurrentBalance
        {
            get => _currentBalance;
            set
            {
                _currentBalance = value;
                SaveBalance();
            }
        }

        /// <summary>
        /// Thêm currency với optional source position cho animation.
        /// </summary>
        /// <param name="amount">Số lượng cần thêm</param>
        /// <param name="position">Vị trí nguồn để spawn animation</param>
        public void Add(int amount, Vector3 position = default)
        {
            // Validation: clamp số âm về 0
            if (amount < 0)
            {
                Debug.LogWarning($"[{CurrencyId}] Add amount is negative ({amount}), clamping to 0");
                amount = 0;
            }

            if (amount == 0) return;

            int oldValue = CurrentBalance;
            CurrentBalance += amount;

            OnCurrencyChanged?.Invoke(oldValue, CurrentBalance);
            OnCurrencyAdded?.Invoke(amount, position);
            OnAdded(amount, position);
        }

        /// <summary>
        /// Trừ currency. Balance không bao giờ xuống dưới 0.
        /// </summary>
        /// <param name="amount">Số lượng cần trừ</param>
        public void Subtract(int amount)
        {
            // Validation: clamp số âm về 0
            if (amount < 0)
            {
                Debug.LogWarning($"[{CurrencyId}] Subtract amount is negative ({amount}), ignoring");
                return;
            }

            if (amount == 0) return;

            int oldValue = CurrentBalance;
            CurrentBalance = Mathf.Max(0, CurrentBalance - amount);

            OnCurrencyChanged?.Invoke(oldValue, CurrentBalance);
            OnCurrencySubtracted?.Invoke(amount);
            OnSubtracted(amount);
        }

        /// <summary>
        /// Lấy balance hiện tại.
        /// </summary>
        /// <returns>Balance hiện tại</returns>
        public int Get()
        {
            return CurrentBalance;
        }

        /// <summary>
        /// Đặt balance thành giá trị cụ thể.
        /// </summary>
        /// <param name="amount">Giá trị mới</param>
        /// <param name="position">Optional source position cho animation</param>
        public void Set(int amount, Vector3 position = default)
        {
            // Validation: clamp số âm về 0
            if (amount < 0)
            {
                Debug.LogWarning($"[{CurrencyId}] Set amount is negative ({amount}), clamping to 0");
                amount = 0;
            }

            int oldValue = CurrentBalance;
            CurrentBalance = amount;

            OnCurrencyChanged?.Invoke(oldValue, CurrentBalance);

            // Trigger appropriate event dựa vào change
            if (amount > oldValue)
            {
                OnCurrencyAdded?.Invoke(amount - oldValue, position);
                OnAdded(amount - oldValue, position);
            }
            else if (amount < oldValue)
            {
                OnCurrencySubtracted?.Invoke(oldValue - amount);
                OnSubtracted(oldValue - amount);
            }

            OnChanged(oldValue, CurrentBalance);
        }



        /// <summary>
        /// Hook được gọi sau khi currency được thêm.
        /// Override trong derived class để customize behavior.
        /// </summary>
        protected virtual void OnAdded(int amount, Vector3 position) { }

        /// <summary>
        /// Hook được gọi sau khi currency bị trừ.
        /// Override trong derived class để customize behavior.
        /// </summary>
        protected virtual void OnSubtracted(int amount) { }

        /// <summary>
        /// Hook được gọi sau khi currency được set.
        /// Override trong derived class để customize behavior.
        /// </summary>
        protected virtual void OnChanged(int oldValue, int newValue) { }



        /// <summary>
        /// Lưu balance vào GameData.
        /// Override để customize save behavior.
        /// </summary>
        protected virtual void SaveBalance()
        {
            GameData.Set(StorageKey, CurrentBalance);
            GameData.Save();
        }

        /// <summary>
        /// Load balance từ GameData.
        /// Override để customize load behavior.
        /// </summary>
        protected virtual void LoadBalance()
        {
            _currentBalance = GameData.Get(StorageKey, 0);
        }

        /// <summary>
        /// Khởi tạo balance với giá trị mặc định, chỉ áp dụng nếu chưa từng được lưu.
        /// Gọi method này trong Awake/Start của derived class hoặc từ game initializer.
        /// </summary>
        /// <param name="initialValue">Giá trị khởi tạo</param>
        public void InitBalance(int initialValue)
        {
            if (initialValue < 0)
            {
                Debug.LogWarning($"[{CurrencyId}] InitBalance value is negative ({initialValue}), clamping to 0");
                initialValue = 0;
            }
            int oldValue = CurrentBalance;
            _currentBalance = initialValue;
            OnCurrencyChanged?.Invoke(oldValue, _currentBalance);
            SaveBalance();
            Debug.Log($"[{CurrencyId}] Balance initialized to {initialValue}");
        }

    }
}
