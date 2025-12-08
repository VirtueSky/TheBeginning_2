#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using VirtueSky.DataStorage;
using VirtueSky.Misc;
using VirtueSky.UtilsEditor;

[InitializeOnLoad]
public class TheBeginning2WindowEditor : EditorWindow
{
    private Editor _editorGameConfig;
    private GameConfig _gameConfig;
    private Vector2 _scrollPosition;

    [MenuItem("TheBeginning_2/Open GameConfig %`", priority = 1)]
    public static void OpenGameConfigWindow()
    {
        GameConfig gameConfig = FileExtension.FindAssetAtFolder<GameConfig>(new string[] { "Assets" }).FirstOrDefault();
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


    [MenuItem("TheBeginning_2/Gameplay %F3", priority = 103)]
    public static void PlayFromGamePlayScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.GAMEPLAY_SCENE}.unity");
        Debug.Log($"Change {Constant.GAMEPLAY_SCENE} scene succeed".SetColor(Color.cyan));
    }

    [MenuItem("TheBeginning_2/Service %F2", priority = 102)]
    public static void PlayFromServiceScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.SERVICE_SCENE}.unity");
        Debug.Log($"Change {Constant.SERVICE_SCENE} scene succeed".SetColor(Color.cyan));
    }

    [MenuItem("TheBeginning_2/Launcher %F1", priority = 101)]
    public static void PlayFromLauncherScene()
    {
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.LAUNCHER_SCENE}.unity");
        Debug.Log($"Change {Constant.SERVICE_SCENE} scene succeed".SetColor(Color.cyan));
    }

    static TheBeginning2WindowEditor()
    {
        ToolbarExtender.LeftToolbarGUI.Add(() =>
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Launcher Scene"))
            {
                PlayFromLauncherScene();
            }

            if (GUILayout.Button("Service Scene"))
            {
                PlayFromServiceScene();
            }

            if (GUILayout.Button("Game Scene"))
            {
                PlayFromGamePlayScene();
            }
        });

        ToolbarExtender.RightToolbarGUI.Add(() =>
        {
            if (GUILayout.Button("Game Config"))
            {
                OpenGameConfigWindow();
            }

            if (GUILayout.Button("Clear All Data"))
            {
                DataWindowEditor.ClearAllData();
            }

            GUILayout.FlexibleSpace();
        });
    }
}
#endif