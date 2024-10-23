using Base.Global;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigs;
using Cysharp.Threading.Tasks;
using VirtueSky.Localization;

namespace Base.Launcher
{
    [EditorIcon("icon_manager")]
    public class LoadingManager : BaseMono
    {
        [HeaderLine("Attributes")] public Image progressBar;
        public LocaleTextComponent localeTextLoading;
        [Range(0.1f, 10f)] public float timeLoading = 5f;
        [SerializeField] bool isWaitingFetchRemoteConfig = true;
        private bool flagDoneProgress;

        private void Awake()
        {
            Init();
            LoadScene();
        }


        private void Init()
        {
            Locale.LoadLanguageSetting();
            progressBar.fillAmount = 0;
            progressBar.DOFillAmount(1, timeLoading)
                .OnUpdate(progressBar,
                    (image, tween) => localeTextLoading.UpdateArgs($"{(int)(progressBar.fillAmount * 100)}"))
                .OnComplete(() => flagDoneProgress = true);
        }

        private async void LoadScene()
        {
            await SceneManager.LoadSceneAsync(Constant.SERVICES_SCENE, LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => flagDoneProgress);
            if (isWaitingFetchRemoteConfig)
            {
                await UniTask.WaitUntil(() => FirebaseRemoteConfigManager.IsFetchRemoteConfigCompleted);
            }

            SceneLoader.Instance.ChangeScene(Constant.GAMEPLAY_SCENE);
        }
    }
}