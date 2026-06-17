using UnityEngine;

namespace Base.Global.Currency
{
    /// <summary>
    /// Diamond currency system.
    /// Ví dụ implementation cho currency type mới.
    /// </summary>
    public class DiamondSystem : BaseCurrencySystem<DiamondSystem>
    {

        protected override CurrencyType CurrencyId => CurrencyType.Diamond;
        protected override string StorageKey => "CURRENT_DIAMOND";
        protected override string DisplayName => "Diamonds";



        /// <summary>
        /// Thêm diamond với optional source position cho animation.
        /// </summary>
        public void AddDiamond(int value, Vector3 pos = default)
        {
            Add(value, pos);
        }

        /// <summary>
        /// Trừ diamond.
        /// </summary>
        public void SubtractDiamond(int value)
        {
            Subtract(value);
        }

        /// <summary>
        /// Lấy số diamond hiện tại.
        /// </summary>
        public int GetCurrentDiamond()
        {
            return Get();
        }

    }
}
