using Base.Game;
using Base.Services;
using Base.UI;
using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.RemoteConfigs;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VirtueSky.Localization;
using VirtueSky.Pattern;
using VirtueSky.Tweening;

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
            DontDestroyOnLoad(gameObject);
            Init();
            LoadScene();
            EventName.PopupHomeShowed.AddListener(DestroyLoadingObject);
        }

        void DestroyLoadingObject()
        {
            EventName.PopupHomeShowed.RemoveListener(DestroyLoadingObject);
            Destroy(gameObject);
        }

        void Init()
        {
            progressBar.fillAmount = 0;
            Tween.Create(progressBar.fillAmount, 1, timeLoading).OnValueChanged(value =>
            {
                progressBar.fillAmount = value;
                localeTextLoading.UpdateArgs($"{(int)(progressBar.fillAmount * 100)}");
            }).WithOnComplete(() => isProgressDone = true).BindToFillAmount(progressBar);
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