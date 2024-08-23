using UnityEngine;
using UnityEngine.AddressableAssets;
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
            case Constant.LAUNCHER_SCENE:
                return;
            case Constant.SERVICES_SCENE:
                await Addressables.LoadSceneAsync(Constant.LAUNCHER_SCENE);
                break;
            case Constant.GAMEPLAY_SCENE:
                await Addressables.LoadSceneAsync(Constant.LAUNCHER_SCENE);
                break;
        }
    }
}