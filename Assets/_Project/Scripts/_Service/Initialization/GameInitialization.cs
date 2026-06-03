using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.Localization;
using VirtueSky.Pattern;

namespace Base.Services
{
    [HideMonoScript]
    public class GameInitialization : ServiceInitialization
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GameObject backgroundLoading;
        public override void Initialization()
        {
            EventName.PopupHomeShowed.AddListener(PopupHomeShowed);
            Application.targetFrameRate = (int)gameConfig.TargetFrameRate;
            Input.multiTouchEnabled = gameConfig.MultiTouchEnabled;
            Locale.LoadLanguageSetting();
        }

        private void PopupHomeShowed()
        {
            EventName.PopupHomeShowed.RemoveListener(PopupHomeShowed);
            App.Delay(0.3f, () => { backgroundLoading.SetActive(false); });
        }
    }
}