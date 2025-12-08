using UnityEngine;
using VirtueSky.Inspector;
using VirtueSky.Localization;

namespace Base.Services
{
    [HideMonoScript]
    public class GameInitialization : ServiceInitialization
    {
        [SerializeField] private GameConfig gameConfig;

        public override void Initialization()
        {
            Application.targetFrameRate = (int)gameConfig.TargetFrameRate;
            Input.multiTouchEnabled = gameConfig.MultiTouchEnabled;
            Locale.LoadLanguageSetting();
        }
    }
}