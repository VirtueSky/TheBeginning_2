using UnityEngine;

namespace Base.Global.Currency
{
    /// <summary>
    /// Diamond display component - hiển thị số diamond trên UI.
    /// Ví dụ implementation cho currency mới.
    /// </summary>
    public class DiamondDisplay : BaseCurrencyDisplay<DiamondSystem, DiamondAnimator>
    {
        protected override BaseCurrencySystem<DiamondSystem> GetCurrencySystem()
        {
            return DiamondSystem.Instance;
        }

        protected override BaseCurrencyAnimator<DiamondAnimator> GetCurrencyAnimator()
        {
            return DiamondAnimator.Instance;
        }
    }
}
