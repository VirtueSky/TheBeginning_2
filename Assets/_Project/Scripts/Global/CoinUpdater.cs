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
        public CoinGenerater coinGenerate;
        [SerializeField] private SoundData soundCoinMove;


        private int _currentCoin;

        public override void OnEnable()
        {
            base.OnEnable();
            Observer.CoinTotalChanged += UpdateCoinAmountText;
            CurrencyAmountText.text = UserData.CoinTotal.ToString();
            SaveCurrentCoin();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Observer.CoinTotalChanged -= UpdateCoinAmountText;
        }

        private void SaveCurrentCoin()
        {
            _currentCoin = UserData.CoinTotal;
        }

        public void UpdateCoinAmountText()
        {
            if (UserData.CoinTotal > _currentCoin)
            {
                IncreaseCoin();
            }
            else
            {
                DecreaseCoin();
            }
        }

        private void IncreaseCoin()
        {
            bool isFirstMove = false;
            coinGenerate.GenerateCoin(() =>
            {
                if (!isFirstMove)
                {
                    isFirstMove = true;
                    AudioManager.Instance.PlaySfx(soundCoinMove);
                    int currentCurrencyAmount = int.Parse(CurrencyAmountText.text);
                    int nextAmount = (UserData.CoinTotal - currentCurrencyAmount) / StepCount;
                    int step = StepCount;
                    CoinTextCount(currentCurrencyAmount, nextAmount, step);
                }
            }, () => { SaveCurrentCoin(); });
        }

        private void DecreaseCoin()
        {
            int currentCurrencyAmount = int.Parse(CurrencyAmountText.text);
            int nextAmount = (UserData.CoinTotal - currentCurrencyAmount) / StepCount;
            int step = StepCount;
            CoinTextCount(currentCurrencyAmount, nextAmount, step);
            SaveCurrentCoin();
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