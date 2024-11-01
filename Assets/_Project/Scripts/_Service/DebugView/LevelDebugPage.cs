using System.Threading.Tasks;
using Base.Data;
using Base.Game;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using System.Collections;

namespace Base.Services
{
    public class LevelDebugPage : DefaultDebugPageBase
    {
        private Sprite iconNext;
        private Sprite iconBack;
        private Sprite iconWin;
        private Sprite iconLose;
        private Sprite iconInput;
        private Sprite iconOk;
        protected override string Title => "Level Debug";

        public void Init(Sprite _iconNext,
            Sprite _iconBack, Sprite _iconWin, Sprite _iconLose, Sprite _iconInput, Sprite _iconOk)
        {
            iconNext = _iconNext;
            iconBack = _iconBack;
            iconWin = _iconWin;
            iconLose = _iconLose;
            iconInput = _iconInput;
            iconOk = _iconOk;
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
        {
            OnInitialize();
            return base.Initialize();
        }
#else
        public override IEnumerator Initialize()
        {
            OnInitialize();
            return base.Initialize();
        }
#endif

        void OnInitialize()
        {
            AddButton("Next Level", clicked: NextLevel, icon: iconNext);
            AddButton("Prev Level", clicked: PrevLevel, icon: iconBack);
            AddButton("Win Level", clicked: WinLevel, icon: iconWin);
            AddButton("Lose Level", clicked: LoseLevel, icon: iconLose);
            AddInputField("Input Level:", valueChanged: ChangeLevel, icon: iconInput);
            AddButton("Jump to level input", clicked: PlayCurrentLevel, icon: iconOk);
        }

        void ChangeLevel(string s)
        {
            if (IsPlayingGame())
            {
                UserData.CurrentLevel = int.Parse(s);
            }
            else
            {
                NotificationInGame.Show("Only works when you play games");
            }
        }

        void PlayCurrentLevel()
        {
            if (IsPlayingGame())
            {
                GameManager.Instance.PlayCurrentLevel();
            }
            else
            {
                NotificationInGame.Show("Only works when you play games");
            }
        }

        void NextLevel()
        {
            if (IsPlayingGame())
            {
                GameManager.Instance.NextLevel();
            }
            else
            {
                NotificationInGame.Show("Only works when you play games");
            }
        }

        void PrevLevel()
        {
            if (IsPlayingGame())
            {
                GameManager.Instance.BackLevel();
            }
            else
            {
                NotificationInGame.Show("Only works when you play games");
            }
        }

        void WinLevel()
        {
            if (IsPlayingGame())
            {
                GameManager.Instance.WinLevel(1);
            }
            else
            {
                NotificationInGame.Show("Only works when you play games");
            }
        }

        void LoseLevel()
        {
            if (IsPlayingGame())
            {
                GameManager.Instance.LoseLevel(1);
            }
            else
            {
                NotificationInGame.Show("Only works when you play games");
            }
        }

        private bool IsPlayingGame()
        {
            return GameManager.Instance.GameState == GameState.PlayingLevel;
        }
    }
}