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
        
        protected virtual void OnEnable()
        {
            // Subscribe to CoinSystem events
            if (CoinSystem.Instance != null)
            {
                CoinSystem.Instance.OnCurrencyAdded += OnCurrencyAdded;
            }
        }

        protected virtual void OnDisable()
        {
            // Unsubscribe from CoinSystem events
            if (CoinSystem.Instance != null)
            {
                CoinSystem.Instance.OnCurrencyAdded -= OnCurrencyAdded;
            }
        }



        private void OnCurrencyAdded(int amount, Vector3 position)
        {
            AnimateCollection(amount, position);
        }

    }
}
