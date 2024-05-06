using Base.Data;
using Base.Global;
using Base.Levels;
using Base.UI;
using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.Misc;
using VirtueSky.TrackingRevenue;

namespace Base.Game
{
    [EditorIcon("icon_gamemanager")]
    public class GameManager : Singleton<GameManager>
    {
        [ReadOnly, SerializeField] private GameState gameState;
        [SerializeField] private Transform levelHolder;

        public GameState GameState => gameState;
        private Level CurrentLevel() => LevelLoader.Instance.CurrentLevel();
        private Level PreviousLevel() => LevelLoader.Instance.PreviousLevel();

        private void Start()
        {
            BackHome();
        }

        public void BackHome()
        {
            gameState = GameState.Lobby;
            PopupManager.Instance.Show<PopupHome>();
            levelHolder.ClearTransform();
        }

        public void PlayCurrentLevel()
        {
            StartLevel();
            PopupManager.Instance.Show<PopupInGame>();
        }

        public void ReplayLevel()
        {
            Observer.OnReplayLevel?.Invoke(CurrentLevel());
            AppTracking.FirebaseAnalyticTrack("On_Replay_Level", "level_name", CurrentLevel().name);
            StartLevel();
            PopupManager.Instance.Show<PopupInGame>();
        }

        public async void NextLevel()
        {
            Observer.OnNextLevel?.Invoke(CurrentLevel());
            UserData.CurrentLevel++;
            var levelPrefab = await LevelLoader.Instance.LoadLevel();
            levelHolder.ClearTransform();
            Instantiate(levelPrefab, levelHolder, false);
        }

        public void BackLevel()
        {
            UserData.CurrentLevel--;
            var levelPrefab = PreviousLevel();
            levelHolder.ClearTransform();
            Instantiate(levelPrefab, levelHolder, false);
            var ins = LevelLoader.Instance.LoadLevel();
        }

        public void StartLevel()
        {
            gameState = GameState.PlayingLevel;
            Observer.OnStartLevel?.Invoke(CurrentLevel());
            var currentLevelPrefab = CurrentLevel();
            levelHolder.ClearTransform();
            Instantiate(currentLevelPrefab, levelHolder, false);
            AppTracking.FirebaseAnalyticTrack("On_Start_Level", "level_name", CurrentLevel().name);
        }

        public void WinLevel(float timeDelayShowPopup = 2.5f)
        {
            if (gameState == GameState.WaitingResult || gameState == GameState.WinLevel ||
                gameState == GameState.LoseLevel) return;
            gameState = GameState.WinLevel;
            Observer.OnWinLevel?.Invoke(CurrentLevel());
            UserData.AdsCounter++;
            AppTracking.FirebaseAnalyticTrack("On_Win_Level", "level_name", CurrentLevel().name);
            App.Delay(timeDelayShowPopup, () =>
            {
                UserData.CurrentLevel++;
                var ins = LevelLoader.Instance.LoadLevel();
                PopupManager.Instance.Show<PopupWin>();
            });
        }

        public void LoseLevel(float timeDelayShowPopup = 2.5f)
        {
            if (gameState == GameState.WaitingResult || gameState == GameState.WinLevel ||
                gameState == GameState.LoseLevel) return;
            gameState = GameState.LoseLevel;
            Observer.OnLoseLevel?.Invoke(CurrentLevel());
            UserData.AdsCounter++;
            AppTracking.FirebaseAnalyticTrack("On_Lose_Level", "level_name", CurrentLevel().name);
            App.Delay(timeDelayShowPopup, () => { PopupManager.Instance.Show<PopupLose>(); });
        }
    }

    public enum GameState
    {
        PrepareLevel,
        PlayingLevel,
        WaitingResult,
        LoseLevel,
        WinLevel,
        Lobby
    }
}