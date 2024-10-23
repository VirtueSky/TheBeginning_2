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
    public class SceneLoader : Singleton<SceneLoader>
    {
        public void ChangeScene(string sceneName)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            foreach (var scene in GetAllLoadedScene())
            {
                if (!scene.name.Equals(Constant.SERVICES_SCENE))
                {
                    SceneManager.UnloadSceneAsync(scene);
                    Static.sceneHolder.Remove(scene.name);
                }
            }

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }


        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Static.sceneHolder.Add(scene.name, scene);
            SceneManager.SetActiveScene(scene);
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