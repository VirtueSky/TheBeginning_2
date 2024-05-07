using UnityEngine;
using VirtueSky.Misc;

namespace Base.Services
{
    public class RequireInternetInitialization : ServiceInitialization
    {
        [SerializeField] private GameConfig gameConfig;

        public override void Initialization()
        {
            if (gameConfig.enableRequireInternet)
            {
                InvokeRepeating(nameof(CheckInternet), gameConfig.timeDelayCheckInternet,
                    gameConfig.timeLoopCheckInternet);
            }
        }

        void CheckInternet()
        {
            Common.CheckInternetConnection(() => { RequireInternet.Instance.Show(false); },
                () => { RequireInternet.Instance.Show(true); });
        }
    }
}