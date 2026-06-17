using System.Globalization;
using TMPro;
using UnityEngine;
using VirtueSky.Tweening;

namespace Base.Global.Currency
{
    /// <summary>
    /// Abstract base class cho tất cả currency display components.
    /// Hiển thị số dư currency trên UI và subscribe vào currency changes.
    /// </summary>
    /// <typeparam name="TSystem">Type của currency system</typeparam>
    /// <typeparam name="TAnimator">Type của currency animator</typeparam>
    public abstract class BaseCurrencyDisplay<TSystem, TAnimator> : MonoBehaviour
        where TSystem : MonoBehaviour
        where TAnimator : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI currencyAmountText;
        [SerializeField] protected GameObject iconTarget;


        /// <summary>
        /// Trả về currency system instance.
        /// Override trong derived class để return specific system.
        /// </summary>
        protected abstract BaseCurrencySystem<TSystem> GetCurrencySystem();

        /// <summary>
        /// Trả về currency animator instance.
        /// Override trong derived class để return specific animator.
        /// </summary>
        protected abstract BaseCurrencyAnimator<TAnimator> GetCurrencyAnimator();


        private bool _isFirstCoinMoveDone = false;
        private bool _isAnimationRunning = false;


        protected virtual void OnEnable()
        {
            // Null check
            if (currencyAmountText == null)
            {
                Debug.LogWarning($"[{GetType().Name}] currencyAmountText is null, cannot display currency");
                return;
            }

            var system = GetCurrencySystem();
            if (system != null)
            {
                // Subscribe vào currency events
                //system.OnCurrencyChanged += OnCurrencyChanged;
                BaseCurrencySystem<TSystem>.OnCurrencyAdded += OnCurrencyAdded;
                BaseCurrencySystem<TSystem>.OnCurrencySubtracted += OnCurrencySubtracted;

                // Hiển thị initial balance (không có animation)
                int currentBalance = system.Get();
                currencyAmountText.text = FormatCurrency(currentBalance);
            }

            var animator = GetCurrencyAnimator();
            if (animator != null && iconTarget != null)
            {
                // Đăng ký animation target
                animator.PushTarget(iconTarget);

                // Subscribe vào animation events
                animator.OnMoveOneCoinDone += OnMoveOneCoinDone;
                animator.OnMoveAllCoinDone += OnMoveAllCoinDone;
            }
            else if (iconTarget == null)
            {
                Debug.LogWarning($"[{GetType().Name}] iconTarget is null, animation target not registered");
            }
        }

        protected virtual void OnDisable()
        {
            var system = GetCurrencySystem();
            if (system != null)
            {
                //system.OnCurrencyChanged -= OnCurrencyChanged;
                BaseCurrencySystem<TSystem>.OnCurrencyAdded -= OnCurrencyAdded;
                BaseCurrencySystem<TSystem>.OnCurrencySubtracted -= OnCurrencySubtracted;
            }

            var animator = GetCurrencyAnimator();
            if (animator != null)
            {
                animator.PopTarget();
                animator.OnMoveOneCoinDone -= OnMoveOneCoinDone;
                animator.OnMoveAllCoinDone -= OnMoveAllCoinDone;
            }
        }


        private void OnCurrencyAdded(int amount, Vector3 position)
        {
            if (position != default)
            {
                // Có animation — đợi OnMoveOneCoinDone mới update display
                _isAnimationRunning = true;
            }
            else
            {
                // Không có animation — update display ngay
                var system = GetCurrencySystem();
                if (system != null) UpdateDisplay(0, system.Get());
            }
        }

        private void OnCurrencyChanged(int oldValue, int newValue)
        {
            // Chỉ update nếu KHÔNG có animation đang chạy
            if (!_isAnimationRunning)
            {
                UpdateDisplay(oldValue, newValue);
            }
        }

        private void OnCurrencySubtracted(int amount)
        {
            // Chỉ update nếu KHÔNG có animation đang chạy
            if (!_isAnimationRunning)
            {
                var system = GetCurrencySystem();
                if (system != null)
                {
                    UpdateDisplay(0, system.Get());
                }
            }
        }

        private void OnMoveOneCoinDone()
        {
            // Update text khi coin ĐẦU TIÊN chạm vào target
            if (!_isFirstCoinMoveDone)
            {
                _isFirstCoinMoveDone = true;
                var system = GetCurrencySystem();
                if (system != null)
                {
                    UpdateDisplay(0, system.Get());
                }
            }
        }

        private void OnMoveAllCoinDone()
        {
            // Reset flags khi tất cả coins done
            _isFirstCoinMoveDone = false;
            _isAnimationRunning = false;
        }


        /// <summary>
        /// Update hiển thị text với smooth tween animation.
        /// </summary>
        /// <param name="oldValue">Giá trị cũ</param>
        /// <param name="newValue">Giá trị mới</param>
        protected virtual void UpdateDisplay(int oldValue, int newValue)
        {
            if (currencyAmountText == null)
            {
                Debug.LogWarning($"[{GetType().Name}] currencyAmountText is null, cannot update display");
                return;
            }

            // Parse current displayed value
            int currentDisplayed = oldValue;
            if (!string.IsNullOrEmpty(currencyAmountText.text))
            {
                // Remove thousand separators trước khi parse
                string cleanText = currencyAmountText.text.Replace(",", "").Replace(".", "");
                if (int.TryParse(cleanText, out int parsed))
                {
                    currentDisplayed = parsed;
                }
            }

            // Nếu không có sự thay đổi, chỉ update text
            if (currentDisplayed == newValue)
            {
                currencyAmountText.text = FormatCurrency(newValue);
                return;
            }

            // Tween từ current → new value over 0.5s
            int tweenValue = currentDisplayed;
            Tween.Create(currentDisplayed, newValue, 0.5f).OnValueChanged(value =>
            {
                if (currencyAmountText != null)
                {
                    currencyAmountText.text = FormatCurrency((int)value);
                }
            }).Play();
        }

        /// <summary>
        /// Format số với thousand separators.
        /// </summary>
        /// <param name="value">Giá trị cần format</param>
        /// <returns>String đã format (ví dụ: "1,000")</returns>
        protected virtual string FormatCurrency(int value)
        {
            return value.ToString("N0", CultureInfo.InvariantCulture);
        }
    }
}