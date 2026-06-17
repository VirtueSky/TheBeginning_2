using UnityEngine;

namespace Base.Global.Currency
{
    /// <summary>
    /// Coin display component - hiển thị số coin trên UI.
    /// Kế thừa từ BaseCurrencyDisplay.
    /// </summary>
    public class CoinDisplay : BaseCurrencyDisplay<CoinSystem, CoinAnimator>
    {
        protected override BaseCurrencySystem<CoinSystem> GetCurrencySystem()
        {
            return CoinSystem.Instance;
        }

        protected override BaseCurrencyAnimator<CoinAnimator> GetCurrencyAnimator()
        {
            return CoinAnimator.Instance;
        }
    }
}
