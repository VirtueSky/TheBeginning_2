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
        public int StepCount = 10;
        public float DelayTime = .01f;
        [SerializeField] private GameObject iconCoin;
        bool isFirsCoinMoveDone = false;

        public override void OnEnable()
        {
            base.OnEnable();
            CurrencyAmountText.text = UserData.CoinTotal.ToString();
            CoinGenerate.Instance.AddTo(iconCoin);
            CoinGenerate.Instance.moveOneCoinDone += MoveOneCoinDone;
            CoinGenerate.Instance.moveAllCoinDone += MoveAllCoinDone;
            CoinGenerate.Instance.decreaseCoin += DecreaseCoin;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CoinGenerate.Instance.RemoveTo(iconCoin);
            CoinGenerate.Instance.moveOneCoinDone -= MoveOneCoinDone;
            CoinGenerate.Instance.moveAllCoinDone -= MoveAllCoinDone;
            CoinGenerate.Instance.decreaseCoin -= DecreaseCoin;
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

        private void CoinTextCount(int currentCurrencyValue, int nextAmountValue, int stepCount)
        {
            if (stepCount == 0)
            {
                CurrencyAmountText.text = UserData.CoinTotal.ToString();
                return;
            }

            int totalValue = (currentCurrencyValue + nextAmountValue);
            DOTween.Sequence().AppendInterval(DelayTime).SetUpdate(isIndependentUpdate: true).AppendCallback(() =>
                {
                    CurrencyAmountText.text = totalValue.ToString();
                })
                .AppendCallback(() => { CoinTextCount(totalValue, nextAmountValue, stepCount - 1); });
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