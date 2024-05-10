using Base.Data;
using PrimeTween;
using TMPro;
using UnityEngine;
using VirtueSky.Audio;
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
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CoinGenerate.Instance.RemoveTo(iconCoin);
            CoinGenerate.Instance.moveOneCoinDone -= MoveOneCoinDone;
            CoinGenerate.Instance.moveAllCoinDone -= MoveAllCoinDone;
        }

        void MoveOneCoinDone()
        {
            if (!isFirsCoinMoveDone)
            {
                isFirsCoinMoveDone = true;
                int currentCurrencyAmount = int.Parse(CurrencyAmountText.text);
                int nextAmount = (UserData.CoinTotal - currentCurrencyAmount) / StepCount;
                int step = StepCount;
                CoinTextCount(currentCurrencyAmount, nextAmount, step);
            }
        }

        void MoveAllCoinDone()
        {
            isFirsCoinMoveDone = false;
        }

        private void DecreaseCoin()
        {
            int currentCurrencyAmount = int.Parse(CurrencyAmountText.text);
            int nextAmount = (UserData.CoinTotal - currentCurrencyAmount) / StepCount;
            int step = StepCount;
            CoinTextCount(currentCurrencyAmount, nextAmount, step);
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
    }
}