using System;
using Base.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VirtueSky.Core;
using VirtueSky.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace Base.Levels
{
    [EditorIcon("icon_controller"), HideMonoScript]
    public class LevelLoader : MonoBehaviour
    {
        [ReadOnly] [SerializeField] private Level currentLevel;
        [ReadOnly] [SerializeField] private Level previousLevel;

        [FormerlySerializedAs("gameConfig")] [SerializeField]
        private GameSettings gameSettings;

        private static event Func<Level> OnGetCurrentLevelEvent;
        private static event Func<Level> OnGetPrevLevelEvent;
        private static event Func<UniTask<Level>> OnLoadLevelEvent;
        private Level InternalCurrentLevel() => currentLevel;
        private Level InternalPreviousLevel() => previousLevel;


        public static Level CurrentLevel() => OnGetCurrentLevelEvent?.Invoke();
        public static Level PreviousLevel() => OnGetPrevLevelEvent?.Invoke();
        public static UniTask<Level> LoadLevel() => (UniTask<Level>)OnLoadLevelEvent?.Invoke();

        private void OnEnable()
        {
            OnGetCurrentLevelEvent += InternalCurrentLevel;
            OnGetPrevLevelEvent += InternalPreviousLevel;
            OnLoadLevelEvent += InternalLoadLevel;
        }

        private void OnDisable()
        {
            OnGetCurrentLevelEvent -= InternalCurrentLevel;
            OnGetPrevLevelEvent -= InternalPreviousLevel;
            OnLoadLevelEvent -= InternalLoadLevel;
        }

        private void Start()
        {
            var instance = InternalLoadLevel();
        }

        private async UniTask<Level> InternalLoadLevel()
        {
            int index = HandleIndexLevel(UserData.CurrentLevel);
            var result = await Addressables.LoadAssetAsync<GameObject>($"Level {index}");
            if (currentLevel != null)
            {
                previousLevel = currentLevel;
            }
            else
            {
                int indexPrev = HandleIndexLevel(UserData.CurrentLevel - 1);
                var resultPre = await Addressables.LoadAssetAsync<GameObject>($"Level {indexPrev}");
                previousLevel = resultPre.GetComponent<Level>();
            }

            currentLevel = result.GetComponent<Level>();
            return currentLevel;
        }

        int HandleIndexLevel(int indexLevel)
        {
            if (indexLevel > gameSettings.maxLevel)
            {
                return (indexLevel - gameSettings.startLoopLevel) %
                       (gameSettings.maxLevel - gameSettings.startLoopLevel + 1) +
                       gameSettings.startLoopLevel;
            }

            if (indexLevel > 0 && indexLevel <= gameSettings.maxLevel)
            {
                //return (indexLevel - 1) % gameConfig.maxLevel + 1;
                return indexLevel;
            }

            if (indexLevel == 0)
            {
                return gameSettings.maxLevel;
            }

            return 1;
        }

        public void ActiveCurrentLevel(bool active)
        {
            currentLevel.gameObject.SetActive(active);
        }
    }
}