#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VirtueSky.ControlPanel.Editor;
using VirtueSky.Misc;


public class TheBeginning2WindowEditor : EditorWindow
{
    enum StateWindow
    {
        GameSettings,
        LevelSettings,
        PopupSettings
    }

    private StateWindow stateWindow;
    private Vector2 scrollButton = Vector2.zero;

    private Editor _editorGameConfig;
    private GameSettings gameSettings;
    private Vector2 _scrollPosition;

    [MenuItem("TheBeginning_2/Open GameConfig %`", priority = 1)]
    public static void OpenGameConfigWindow()
    {
        TheBeginning2WindowEditor window = GetWindow<TheBeginning2WindowEditor>("The Beginning");
        if (window == null)
        {
            Debug.LogError("Couldn't open the TheBeginning window!");
            return;
        }

        window.minSize = new Vector2(550, 500);
        window.Show();
    }

    private void OnEnable()
    {
        GameSettingsWindow.OnEnable();
        LevelSettingsWindow.OnEnable();
        PopupSettingsWindow.OnEnable();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        CPUtility.DrawHeader("TheBeginning", 17);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        Handles.color = Color.black;
        CPUtility.DrawCustomLine(4, new Vector2(0, 30), new Vector2(position.width, 30));
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(165));
        scrollButton = EditorGUILayout.BeginScrollView(scrollButton);
        DrawButton();
        EditorGUILayout.EndScrollView();
        CPUtility.DrawCustomLine(4, new Vector3(170, 30), new Vector3(170, position.height));
        GUILayout.EndVertical();
        GUILayout.Space(10);
        DrawContent();
        GUILayout.EndHorizontal();
    }

    private void DrawContent()
    {
        switch (stateWindow)
        {
            case StateWindow.GameSettings:
                GameSettingsWindow.Draw();
                break;
            case StateWindow.LevelSettings:
                LevelSettingsWindow.Draw();
                break;
            case StateWindow.PopupSettings:
                PopupSettingsWindow.Draw();
                break;
        }
    }

    private void DrawButton()
    {
        DrawButtonChooseState("Game Settings", StateWindow.GameSettings);
        DrawButtonChooseState("Level Settings", StateWindow.LevelSettings);
        DrawButtonChooseState("Popup Settings", StateWindow.PopupSettings);
    }

    void DrawButtonChooseState(string title, StateWindow _stateWindow)
    {
        bool clicked = GUILayout.Toggle(stateWindow == _stateWindow, title, GUI.skin.button,
            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(20));
        if (clicked && stateWindow != _stateWindow)
        {
            stateWindow = _stateWindow;
        }

        GUILayout.Space(2);
    }

    [MenuItem("TheBeginning_2/Gameplay %F2", priority = 102)]
    public static void PlayFromGamePlayScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.GAMEPLAY_SCENE}.unity");
        Debug.Log($"Change {Constant.GAMEPLAY_SCENE} scene succeed".SetColor(Color.cyan));
    }

    [MenuItem("TheBeginning_2/Service %F1", priority = 101)]
    public static void PlayFromServiceScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.SERVICES_SCENE}.unity");
        Debug.Log($"Change {Constant.SERVICES_SCENE} scene succeed".SetColor(Color.cyan));
    }
}
#endif