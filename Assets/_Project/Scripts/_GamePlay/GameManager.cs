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

        private Level CurrentLevel() => LevelLoader.CurrentLevel();
        private Level PreviousLevel() => LevelLoader.PreviousLevel();

        public static event Action<Level> OnStartLevel;
        public static event Action<Level> OnReplayLevel;
        public static event Action<Level> OnNextLevel;
        public static event Action<Level> OnBackLevel;
        public static event Action<Level> OnWinLevel;
        public static event Action<Level> OnLoseLevel;
        public static event Action<GameState> OnChangeStateGame;


        private void Start()
        {
            BackHome();
        }

        public void BackHome()
        {
            GameState = GameState.Lobby;
            PopupManager.Show<HomePopup>();
            levelHolder.ClearTransform();
        }

        public void PlayCurrentLevel()
        {
            StartLevel();
            PopupManager.Show<GameplayPopup>();
        }

        public void ReplayLevel()
        {
            OnReplayLevel?.Invoke(CurrentLevel());
            StartLevel();
            PopupManager.Show<GameplayPopup>();
        }

        public void NextLevel()
        {
            UserData.CurrentLevel++;
            var levelPrefab = LevelLoader.LoadLevel();
            levelHolder.ClearTransform();
            Instantiate(levelPrefab, levelHolder, false);
            OnNextLevel?.Invoke(CurrentLevel());
        }

        public void BackLevel()
        {
            UserData.CurrentLevel--;
            var levelPrefab = PreviousLevel();
            levelHolder.ClearTransform();
            Instantiate(levelPrefab, levelHolder, false);
            OnBackLevel?.Invoke(CurrentLevel());
            var ins = LevelLoader.LoadLevel();
        }

        public void StartLevel()
        {
            GameState = GameState.PlayingLevel;
            var currentLevelPrefab = CurrentLevel();
            levelHolder.ClearTransform();
            Instantiate(currentLevelPrefab, levelHolder, false);
            OnStartLevel?.Invoke(CurrentLevel());
        }

        public void WinLevel(float timeDelayShowPopup = 2.5f)
        {
            if (GameState == GameState.WaitingResult || GameState == GameState.WinLevel ||
                GameState == GameState.LoseLevel) return;
            GameState = GameState.WinLevel;
            OnWinLevel?.Invoke(CurrentLevel());
            App.Delay(timeDelayShowPopup, () =>
            {
                PopupManager.Show<WinPopup>(showPopupCompleted: () =>
                {
                    UserData.CurrentLevel++;
                    var ins = LevelLoader.LoadLevel();
                });
            });
        }

        public void LoseLevel(float timeDelayShowPopup = 2.5f)
        {
            if (GameState == GameState.WaitingResult || GameState == GameState.WinLevel ||
                GameState == GameState.LoseLevel) return;
            GameState = GameState.LoseLevel;
            OnLoseLevel?.Invoke(CurrentLevel());
            App.Delay(timeDelayShowPopup, () => { PopupManager.Show<LosePopup>(); });
        }

        public GameState GameState
        {
            get => gameState;
            private set
            {
                gameState = value;
                OnChangeStateGame?.Invoke(gameState);
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