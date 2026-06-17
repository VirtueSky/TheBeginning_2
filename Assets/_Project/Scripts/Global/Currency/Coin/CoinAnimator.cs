using UnityEngine;
using VirtueSky.Audio;

namespace Base.Global.Currency
{
    /// <summary>
    /// Coin animator - quản lý animation thu thập coin.
    /// Kế thừa từ BaseCurrencyAnimator.
    /// </summary>
    public class CoinAnimator : BaseCurrencyAnimator<CoinAnimator>
    {
        [SerializeField] private GameObject iconCoinPrefab;
        [SerializeField] private SoundData coinCollectSound;
        [SerializeField] private SoundData coinSpawnSound;
        protected override GameObject AnimationPrefab => iconCoinPrefab;

        protected override SoundData CollectSound => coinCollectSound;
        protected override SoundData SpawnSound => coinSpawnSound;

        public override void OnEnable()
        {
            base.OnEnable();
            CoinSystem.OnCurrencyAdded += OnCurrencyAdded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CoinSystem.OnCurrencyAdded -= OnCurrencyAdded;
        }


        private void OnCurrencyAdded(int amount, Vector3 position)
        {
            AnimateCollection(amount, position);
        }
    }
}