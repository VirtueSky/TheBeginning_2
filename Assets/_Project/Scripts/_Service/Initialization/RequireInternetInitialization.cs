using UnityEngine;
using VirtueSky.Inspector;
using VirtueSky.Misc;

namespace Base.Services
{
    [HideMonoScript]
    public class RequireInternetInitialization : ServiceInitialization
    {
        [SerializeField] private GameConfig gameConfig;

        public override void Initialization()
        {
            if (gameConfig.EnableRequireInternet)
            {
                InvokeRepeating(nameof(CheckInternet), gameConfig.TimeDelayCheckInternet,
                    gameConfig.TimeLoopCheckInternet);
            }
        }

        void CheckInternet()
        {
            Common.CheckInternetConnection(() => { RequireInternet.Instance.Show(false); },
                () => { RequireInternet.Instance.Show(true); });
        }
    }
}