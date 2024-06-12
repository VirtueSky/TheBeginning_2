using System.Threading.Tasks;
using Base.Data;
using Base.Game;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace Base.Services
{
    public class LevelDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "Level Debug";

        public override Task Initialize()
        {
            AddButton("Next Level", clicked: NextLevel, icon: DebugViewStatic.IconNextDebug);
            AddButton("Prev Level", clicked: PrevLevel, icon: DebugViewStatic.IconBackDebug);
            AddButton("Win Level", clicked: WinLevel, icon: DebugViewStatic.IconWinDebug);
            AddButton("Lose Level", clicked: LoseLevel, icon: DebugViewStatic.IconLoseDebug);
            AddInputField("Input Level:", valueChanged: ChangeLevel, icon: DebugViewStatic.IconInputDebug);
            AddButton("Jump to level input", clicked: PlayCurrentLevel, icon: DebugViewStatic.IconOkeDebug);
            return base.Initialize();
        }

        void ChangeLevel(string s)
        {
            if (IsPlayingGame())
            {
                UserData.CurrentLevel = int.Parse(s);
            }
            else
            {
                NotificationInGame.Instance.Show("Only works when you play games");
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
                NotificationInGame.Instance.Show("Only works when you play games");
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
                NotificationInGame.Instance.Show("Only works when you play games");
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
                NotificationInGame.Instance.Show("Only works when you play games");
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
                NotificationInGame.Instance.Show("Only works when you play games");
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
                NotificationInGame.Instance.Show("Only works when you play games");
            }
        }

        private bool IsPlayingGame()
        {
            return GameManager.Instance.GameState == GameState.PlayingLevel;
        }
    }
}