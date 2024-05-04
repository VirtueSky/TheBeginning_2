using VirtueSky.Core;
using VirtueSky.Inspector;

namespace Base.Game
{
    [EditorIcon("icon_gamemanager")]
    public class GameManager : Singleton<GameManager>
    {
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