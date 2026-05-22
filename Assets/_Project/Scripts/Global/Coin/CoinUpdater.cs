using Base.Data;
using TMPro;
using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Tweening;

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
            CurrencyAmountText.text = CoinSystem.GetCurrentCoin().ToString();
            if (CoinGenerate.Instance != null)
            {
                CoinGenerate.Instance.AddTo(iconCoin);
                CoinGenerate.OnMoveOneCoinDone += MoveOneCoinDone;
                CoinGenerate.OnMoveAllCoinDone += MoveAllCoinDone;
                CoinGenerate.OnDecreaseCoin += DecreaseCoin;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (CoinGenerate.Instance != null)
            {
                CoinGenerate.Instance.RemoveTo(iconCoin);
                CoinGenerate.OnMoveOneCoinDone -= MoveOneCoinDone;
                CoinGenerate.OnMoveAllCoinDone -= MoveAllCoinDone;
                CoinGenerate.OnDecreaseCoin -= DecreaseCoin;
            }
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
            
            Tween.Create(starCoin, CoinSystem.GetCurrentCoin(), 0.5f).OnValueChanged(value =>
            {
                CurrencyAmountText.text = ((int)value).ToString();
            });
            
        }
    }
}