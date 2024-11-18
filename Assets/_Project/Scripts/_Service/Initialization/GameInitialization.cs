using UnityEngine;
using UnityEngine.SceneManagement;
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
            Application.targetFrameRate = (int)gameConfig.targetFrameRate;
            Input.multiTouchEnabled = gameConfig.multiTouchEnabled;
            Locale.LoadLanguageSetting();
            SceneManager.LoadSceneAsync(Constant.GAMEPLAY_SCENE, LoadSceneMode.Additive);
        }
    }
}