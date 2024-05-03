using UnityEngine;
using VirtueSky.Inspector;

namespace Base.Services
{
    [HideMonoScript]
    public class FrameRateInitialization : ServiceInitialization
    {
        [SerializeField] private GameConfig gameConfig;

        public override void Initialization()
        {
            Application.targetFrameRate = (int)gameConfig.targetFrameRate;
        }
    }
}