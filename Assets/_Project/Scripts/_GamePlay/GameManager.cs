using System;
using Base.Data;
using Base.Global;
using Base.Levels;
using Base.UI;
using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Inspector;
using VirtueSky.Misc;
using VirtueSky.Tracking;

namespace Base.Game
{
    [EditorIcon("icon_gamemanager")]
    public class GameManager : Singleton<GameManager>
    {
        [ReadOnly, SerializeField] private GameState gameState;
        [SerializeField] private Transform levelHolder;

        private Level CurrentLevel() => LevelLoader.Instance.CurrentLevel();
        private Level PreviousLevel() => LevelLoader.Instance.PreviousLevel();

        public static event Action<Level> OnStartLevel;
        public static event Action<Level> OnReplayLevel;
        public static event Action<Level> OnNextLevel;
        public static event Action<Level> OnBackLevel;
        public static event Action<Level> OnWinLevel;
        public static event Action<Level> OnLoseLevel;

        private void Start()
        {
            BackHome();
        }

        public void BackHome()
        {
            GameState = GameState.Lobby;
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
            OnReplayLevel?.Invoke(CurrentLevel());
            FirebaseTracking.TrackEvent("On_Replay_Level", "level_name", CurrentLevel().name);
            StartLevel();
            PopupManager.Instance.Show<PopupInGame>();
        }

        public async void NextLevel()
        {
            UserData.CurrentLevel++;
            OnNextLevel?.Invoke(CurrentLevel());
            var levelPrefab = await LevelLoader.Instance.LoadLevel();
            levelHolder.ClearTransform();
            Instantiate(levelPrefab, levelHolder, false);
        }

        public void BackLevel()
        {
            UserData.CurrentLevel--;
            OnBackLevel?.Invoke(CurrentLevel());
            var levelPrefab = PreviousLevel();
            levelHolder.ClearTransform();
            Instantiate(levelPrefab, levelHolder, false);
            var ins = LevelLoader.Instance.LoadLevel();
        }

        public void StartLevel()
        {
            GameState = GameState.PlayingLevel;
            OnStartLevel?.Invoke(CurrentLevel());
            var currentLevelPrefab = CurrentLevel();
            levelHolder.ClearTransform();
            Instantiate(currentLevelPrefab, levelHolder, false);
            FirebaseTracking.TrackEvent("On_Start_Level", "level_name", CurrentLevel().name);
        }

        public void WinLevel(float timeDelayShowPopup = 2.5f)
        {
            if (GameState == GameState.WaitingResult || GameState == GameState.WinLevel ||
                GameState == GameState.LoseLevel) return;
            GameState = GameState.WinLevel;
            OnWinLevel?.Invoke(CurrentLevel());
            UserData.AdsCounter++;
            FirebaseTracking.TrackEvent("On_Win_Level", "level_name", CurrentLevel().name);
            App.Delay(timeDelayShowPopup, () =>
            {
                UserData.CurrentLevel++;
                var ins = LevelLoader.Instance.LoadLevel();
                PopupManager.Instance.Show<PopupWin>();
            });
        }

        public void LoseLevel(float timeDelayShowPopup = 2.5f)
        {
            if (GameState == GameState.WaitingResult || GameState == GameState.WinLevel ||
                GameState == GameState.LoseLevel) return;
            GameState = GameState.LoseLevel;
            OnLoseLevel?.Invoke(CurrentLevel());
            UserData.AdsCounter++;
            FirebaseTracking.TrackEvent("On_Lose_Level", "level_name", CurrentLevel().name);
            App.Delay(timeDelayShowPopup, () => { PopupManager.Instance.Show<PopupLose>(); });
        }

        public GameState GameState
        {
            get => gameState;
            set
            {
                gameState = value;
                Observer.OnChangeStateGame?.Invoke(gameState);
            }
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