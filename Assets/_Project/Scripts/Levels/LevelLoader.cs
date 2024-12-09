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
        private LevelSettings levelSettings;

        private static event Func<Level> OnGetCurrentLevelEvent;
        private static event Func<Level> OnGetPrevLevelEvent;
        private static event Func<Level> OnLoadLevelEvent;
        private Level InternalCurrentLevel() => currentLevel;
        private Level InternalPreviousLevel() => previousLevel;


        public static Level CurrentLevel() => OnGetCurrentLevelEvent?.Invoke();
        public static Level PreviousLevel() => OnGetPrevLevelEvent?.Invoke();
        public static Level LoadLevel() => OnLoadLevelEvent?.Invoke();

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

        private Level InternalLoadLevel()
        {
            int index = HandleIndexLevel(UserData.CurrentLevel);
            var result = levelSettings.GePrefabLevel($"Level {index}");
            if (currentLevel != null)
            {
                previousLevel = currentLevel;
            }
            else
            {
                int indexPrev = HandleIndexLevel(UserData.CurrentLevel - 1);
                var resultPre = levelSettings.GePrefabLevel($"Level {indexPrev}");
                previousLevel = resultPre.GetComponent<Level>();
            }

            currentLevel = result.GetComponent<Level>();
            return currentLevel;
        }

        int HandleIndexLevel(int indexLevel)
        {
            if (indexLevel > levelSettings.MaxLevel)
            {
                return (indexLevel - levelSettings.StartLoopLevel) %
                       (levelSettings.MaxLevel - levelSettings.StartLoopLevel + 1) +
                       levelSettings.StartLoopLevel;
            }

            if (indexLevel > 0 && indexLevel <= levelSettings.MaxLevel)
            {
                return indexLevel;
            }

            if (indexLevel == 0)
            {
                return levelSettings.MaxLevel;
            }

            return 1;
        }

        public void ActiveCurrentLevel(bool active)
        {
            currentLevel.gameObject.SetActive(active);
        }
    }
}