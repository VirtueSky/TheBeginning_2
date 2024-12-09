using UnityEngine;
using UnityEngine.Serialization;
using VirtueSky.Inspector;
using VirtueSky.Misc;

namespace Base.Services
{
    [HideMonoScript]
    public class RequireInternetInitialization : ServiceInitialization
    {
        [FormerlySerializedAs("gameConfig")] [SerializeField]
        private GameSettings gameSettings;

        public override void Initialization()
        {
            if (gameSettings.enableRequireInternet)
            {
                InvokeRepeating(nameof(CheckInternet), gameSettings.timeDelayCheckInternet,
                    gameSettings.timeLoopCheckInternet);
            }
        }

        void CheckInternet()
        {
            Common.CheckInternetConnection(() => { RequireInternet.Instance.Show(false); },
                () => { RequireInternet.Instance.Show(true); });
        }
    }
}