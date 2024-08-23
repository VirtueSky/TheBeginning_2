using Base.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VirtueSky.Core;
using VirtueSky.Inspector;
using Cysharp.Threading.Tasks;

namespace Base.Levels
{
    [EditorIcon("icon_controller"), HideMonoScript]
    public class LevelLoader : Singleton<LevelLoader>
    {
        [ReadOnly] [SerializeField] private Level currentLevel;
        [ReadOnly] [SerializeField] private Level previousLevel;
        [SerializeField] private GameConfig gameConfig;

        public Level CurrentLevel() => currentLevel;
        public Level PreviousLevel() => previousLevel;

        private void Start()
        {
            var instance = LoadLevel();
        }

        public async UniTask<Level> LoadLevel()
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
            if (indexLevel > gameConfig.maxLevel)
            {
                return (indexLevel - gameConfig.startLoopLevel) %
                       (gameConfig.maxLevel - gameConfig.startLoopLevel + 1) +
                       gameConfig.startLoopLevel;
            }

            if (indexLevel > 0 && indexLevel <= gameConfig.maxLevel)
            {
                //return (indexLevel - 1) % gameConfig.maxLevel + 1;
                return indexLevel;
            }

            if (indexLevel == 0)
            {
                return gameConfig.maxLevel;
            }

            return 1;
        }

        public void ActiveCurrentLevel(bool active)
        {
            currentLevel.gameObject.SetActive(active);
        }
    }
}