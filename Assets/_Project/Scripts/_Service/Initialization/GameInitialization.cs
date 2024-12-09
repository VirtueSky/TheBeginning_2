using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using VirtueSky.Inspector;
using VirtueSky.Localization;

namespace Base.Services
{
    [HideMonoScript]
    public class GameInitialization : ServiceInitialization
    {
        [FormerlySerializedAs("gameConfig")] [SerializeField]
        private GameSettings gameSettings;

        public override void Initialization()
        {
            Application.targetFrameRate = (int)gameSettings.TargetFrameRate;
            Input.multiTouchEnabled = gameSettings.MultiTouchEnabled;
            Locale.LoadLanguageSetting();
            SceneManager.LoadSceneAsync(Constant.GAMEPLAY_SCENE, LoadSceneMode.Additive);
        }
    }
}