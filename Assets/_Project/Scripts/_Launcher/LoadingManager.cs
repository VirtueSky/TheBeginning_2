using Base.Services;
using PrimeTween;
using UnityEngine;
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

        private void Start()
        {
            progressBar.fillAmount = 0;
            progressBar.DOFillAmount(1, timeLoading)
                .OnUpdate(progressBar,
                    (image, tween) => localeTextLoading.UpdateArgs($"{(int)(progressBar.fillAmount * 100)}"))
                .OnComplete(Done);
        }

        private async void Done()
        {
            if (isWaitingFetchRemoteConfig)
            {
                await UniTask.WaitUntil(() => FirebaseRemoteConfigManager.IsFetchRemoteConfigCompleted);
            }

            NotificationInGame.Show("Welcome!");
            Destroy(gameObject);
        }
    }
}