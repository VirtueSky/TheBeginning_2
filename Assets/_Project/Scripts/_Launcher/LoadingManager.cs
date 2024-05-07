using Base.Global;
using Base.Services;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.Threading.Tasks;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Base.Launcher
{
    [EditorIcon("icon_manager")]
    public class LoadingManager : BaseMono
    {
        [HeaderLine("Attributes")] public Image progressBar;
        public TextMeshProUGUI loadingText;
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
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
            ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif

            progressBar.fillAmount = 0;
            progressBar.DOFillAmount(1, timeLoading)
                .OnUpdate(progressBar,
                    (image, tween) => loadingText.text = $"Loading... {(int)(progressBar.fillAmount * 100)}%")
                .OnComplete(() => flagDoneProgress = true);
        }

        private async void LoadScene()
        {
            await UniTask.WaitUntil(() => flagDoneProgress);
            await Addressables.LoadSceneAsync(Constant.SERVICES_SCENE, LoadSceneMode.Additive);
            if (isWaitingFetchRemoteConfig)
            {
                await UniTask.WaitUntil(() => FirebaseRemoteConfigManager.Instance.IsFetchRemoteConfigCompleted);
            }

            SceneLoader.Instance.ChangeScene(Constant.GAMEPLAY_SCENE);
        }


        void OnServiceLoaded(AsyncOperationHandle<SceneInstance> scene)
        {
            if (scene.Status == AsyncOperationStatus.Succeeded)
            {
                string sceneName = scene.Result.Scene.name;
                Static.sceneHolder.Add(sceneName, scene);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            }
        }
    }
}