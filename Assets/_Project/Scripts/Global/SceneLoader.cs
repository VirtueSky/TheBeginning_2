using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VirtueSky.Core;
using VirtueSky.Inspector;

namespace Base.Global
{
    [EditorIcon("icon_controller"), HideMonoScript]
    public class SceneLoader : MonoBehaviour
    {
        public void ChangeScene(AssetReference sceneReference)
        {
            foreach (var scene in GetAllLoadedScene())
            {
                if (!scene.name.Equals(Constant.SERVICES_SCENE))
                {
                    if (Static.sceneHolder.ContainsKey(scene.name))
                    {
                        Addressables.UnloadSceneAsync(Static.sceneHolder[scene.name]);
                        Static.sceneHolder.Remove(scene.name);
                    }
                    else
                    {
                        SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            Addressables.LoadSceneAsync(sceneReference, LoadSceneMode.Additive).Completed += OnAdditiveSceneLoaded;
        }

        public void ChangeScene(string sceneName)
        {
            foreach (var scene in GetAllLoadedScene())
            {
                if (!scene.name.Equals(Constant.SERVICES_SCENE))
                {
                    if (Static.sceneHolder.ContainsKey(scene.name))
                    {
                        Addressables.UnloadSceneAsync(Static.sceneHolder[scene.name]);
                        Static.sceneHolder.Remove(scene.name);
                    }
                    else
                    {
                        SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive).Completed += OnAdditiveSceneLoaded;
        }

        void OnAdditiveSceneLoaded(AsyncOperationHandle<SceneInstance> scene)
        {
            if (scene.Status == AsyncOperationStatus.Succeeded)
            {
                string sceneName = scene.Result.Scene.name;
                Static.sceneHolder.Add(sceneName, scene);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            }
        }

        private Scene[] GetAllLoadedScene()
        {
            int countLoaded = SceneManager.sceneCount;
            var loadedScenes = new Scene[countLoaded];

            for (var i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }

            return loadedScenes;
        }
    }
}