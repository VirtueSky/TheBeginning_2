using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public struct EditorInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static async void RuntimeEditorInitialize()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case Constant.SERVICES_SCENE:
                return;
            case Constant.GAMEPLAY_SCENE:
                await SceneManager.LoadSceneAsync(Constant.GAMEPLAY_SCENE);
                break;
        }
    }
}