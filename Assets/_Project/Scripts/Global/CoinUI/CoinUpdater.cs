using Base.Data;
using PrimeTween;
using TMPro;
using UnityEngine;
using VirtueSky.Core;

namespace Base.Global
{
    public class CoinUpdater : BaseMono
    {
        public TextMeshProUGUI CurrencyAmountText;
        [SerializeField] private GameObject iconCoin;
        bool isFirsCoinMoveDone = false;

        public override void OnEnable()
        {
            base.OnEnable();
            CurrencyAmountText.text = UserData.CoinTotal.ToString();
            CoinGenerate.Instance.AddTo(iconCoin);
            CoinGenerate.OnMoveOneCoinDone += MoveOneCoinDone;
            CoinGenerate.OnMoveAllCoinDone += MoveAllCoinDone;
            CoinGenerate.OnDecreaseCoin += DecreaseCoin;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CoinGenerate.Instance.RemoveTo(iconCoin);
            CoinGenerate.OnMoveOneCoinDone -= MoveOneCoinDone;
            CoinGenerate.OnMoveAllCoinDone -= MoveAllCoinDone;
            CoinGenerate.OnDecreaseCoin -= DecreaseCoin;
        }

        void MoveOneCoinDone()
        {
            if (!isFirsCoinMoveDone)
            {
                isFirsCoinMoveDone = true;
                UpdateTextCoin();
            }
        }

        void MoveAllCoinDone()
        {
            isFirsCoinMoveDone = false;
        }

        private void DecreaseCoin()
        {
            UpdateTextCoin();
        }


        void UpdateTextCoin()
        {
            int starCoin = int.Parse(CurrencyAmountText.text);
            int coinChange = starCoin;
            Tween.Custom(starCoin, UserData.CoinTotal, 0.5f, valueChange => coinChange = (int)valueChange)
                .OnUpdate(this, (coin, tween) => { CurrencyAmountText.text = coinChange.ToString(); });
        }
    }
}