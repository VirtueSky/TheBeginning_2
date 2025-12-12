using Base.Services;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigs;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
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
        private bool isProgressDone = false;

        private void Awake()
        {
            Init();
            LoadScene();
        }

        void Init()
        {
            progressBar.fillAmount = 0;
            progressBar.DOFillAmount(1, timeLoading)
                .OnUpdate(progressBar,
                    (image, tween) => localeTextLoading.UpdateArgs($"{(int)(progressBar.fillAmount * 100)}"))
                .OnComplete(() => isProgressDone = true, false);
        }

        private async void LoadScene()
        {
            await SceneLoader.Instance.LoadSceneAdditiveAsync(Constant.SERVICE_SCENE);
            await UniTask.WaitUntil(() => isProgressDone);
            App.Delay(1.0f, () => { NotificationInGame.Show("Welcome TheBeginning"); });
            if (isWaitingFetchRemoteConfig)
            {
                await UniTask.WaitUntil(() => FirebaseRemoteConfigManager.IsFetchRemoteConfigCompleted);
            }

            await SceneLoader.Instance.ChangeSceneAsync(Constant.GAMEPLAY_SCENE);
        }
    }
}