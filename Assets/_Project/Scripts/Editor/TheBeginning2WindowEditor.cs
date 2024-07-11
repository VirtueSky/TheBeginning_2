#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VirtueSky.DataStorage;
using VirtueSky.Inspector;
using VirtueSky.UtilsEditor;

public class TheBeginning2WindowEditor : EditorWindow
{
    private Editor _editorGameConfig;
    private GameConfig _gameConfig;
    private Vector2 _scrollPosition;

    [MenuItem("TheBeginning_2/Open GameConfig %`", priority = 1)]
    public static void OpenGameConfigWindow()
    {
        GameConfig gameConfig = AssetUtils.FindAssetAtFolder<GameConfig>(new string[] { "Assets" }).FirstOrDefault();
        TheBeginning2WindowEditor window = GetWindow<TheBeginning2WindowEditor>("Game Config");
        window._gameConfig = gameConfig;
        if (window == null)
        {
            Debug.LogError("Couldn't open the ads settings window!");
            return;
        }

        window.minSize = new Vector2(350, 250);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height),
            GameDataEditor.ColorBackgroundRectWindowSunflower.ToColor());
        GUI.contentColor = GameDataEditor.ColorTextContentWindowSunflower.ToColor();
        GUI.backgroundColor = GameDataEditor.ColorContentWindowSunflower.ToColor();
        if (_editorGameConfig == null)
        {
            _editorGameConfig = UnityEditor.Editor.CreateEditor(_gameConfig);
        }

        if (_editorGameConfig == null)
        {
            EditorGUILayout.HelpBox("Couldn't create the settings resources editor.",
                MessageType.Error);
            return;
        }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        _editorGameConfig.OnInspectorGUI();
        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }

    [MenuItem("TheBeginning_2/Launcher %F1", priority = 101)]
    public static void PlayFromLauncherScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.LAUNCHER_SCENE}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("TheBeginning_2/Gameplay %F2", priority = 102)]
    public static void PlayFromGamePlayScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.GAMEPLAY_SCENE}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("TheBeginning_2/Service %F3", priority = 103)]
    public static void PlayFromServiceScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.SERVICES_SCENE}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }
}
#endif