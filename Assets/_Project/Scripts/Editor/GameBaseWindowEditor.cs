#if UNITY_EDITOR
using Base.Data;
using Base.Game;
using Base.Global;
using Base.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameBaseWindowEditor : EditorWindow
{
    void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.EndHorizontal();
    }

    [MenuItem("GameBase/Switch IsTesting %`")]
    public static void SwitchIsTesting()
    {
        UserData.IsTestingAdministrator = !UserData.IsTestingAdministrator;
        Debug.Log($"<color=Green>Data.IsTesting = {UserData.IsTestingAdministrator}</color>");
    }

    [MenuItem("GameBase/Open Scene/Launcher %F1")]
    public static void PlayFromLauncherScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.LAUNCHER_SCENE}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("GameBase/Open Scene/Gameplay %F3")]
    public static void PlayFromGamePlayScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.GAMEPLAY_SCENE}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("GameBase/Open Scene/Service %F2")]
    public static void PlayFromServiceScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.SERVICES_SCENE}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("GameBase/Data/Add 100k Money")]
    public static void Add100kMoney()
    {
        UserData.CoinTotal += 100000;
        Debug.Log($"<color=Green>Add 100k coin succeed</color>");
    }
}
#endif