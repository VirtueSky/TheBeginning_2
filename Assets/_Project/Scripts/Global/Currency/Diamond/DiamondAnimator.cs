using UnityEngine;
using VirtueSky.Audio;

namespace Base.Global.Currency
{
    /// <summary>
    /// Diamond animator - quản lý animation thu thập diamond.
    /// Ví dụ implementation với custom animation config.
    /// </summary>
    public class DiamondAnimator : BaseCurrencyAnimator<DiamondAnimator>
    {
        [SerializeField] private GameObject diamondPrefab;
        [SerializeField] private SoundData diamondCollectSound;
        protected override GameObject AnimationPrefab => diamondPrefab;

        protected override SoundData CollectSound => diamondCollectSound;



        protected virtual void OnEnable()
        {
            // Subscribe to DiamondSystem events
            DiamondSystem.Instance.OnCurrencyAdded += OnCurrencyAdded;
        }

        protected virtual void OnDisable()
        {
            // Unsubscribe from DiamondSystem events
            if (DiamondSystem.Instance != null)
            {
                DiamondSystem.Instance.OnCurrencyAdded -= OnCurrencyAdded;
            }
        }



        private void OnCurrencyAdded(int amount, Vector3 position)
        {
            AnimateCollection(amount, position);
        }

    }
}
