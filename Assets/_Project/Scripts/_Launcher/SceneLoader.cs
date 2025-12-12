using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VirtueSky.DataType;
using VirtueSky.Pattern;

namespace Base.Launcher
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public DictionaryCustom<string, AsyncOperationHandle<SceneInstance>> sceneHolder =
            new DictionaryCustom<string, AsyncOperationHandle<SceneInstance>>();

        public async UniTask<bool> LoadSceneAdditiveAsync(string sceneName, Action<float> onProgress = null)
        {
            var handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            while (!handle.IsDone)
            {
                onProgress?.Invoke(handle.PercentComplete);
                await UniTask.Yield();
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                string loadedSceneName = handle.Result.Scene.name;
                if (!sceneHolder.ContainsKey(loadedSceneName))
                {
                    sceneHolder.Add(loadedSceneName, handle);
                }
                return true;
            }

            return false;
        }

        public void ChangeScene(string sceneName, Action<float> onProgress = null)
        {
            ChangeSceneAsync(sceneName, onProgress).Forget();
        }

        public async UniTask ChangeSceneAsync(string sceneName, Action<float> onProgress = null)
        {
            // Collect scenes to unload (exclude SERVICE_SCENE)
            var scenesToUnload = new List<string>();
            foreach (var scene in GetAllLoadedScene())
            {
                if (scene.IsValid() && !string.IsNullOrEmpty(scene.name) && !scene.name.Equals(Constant.SERVICE_SCENE))
                {
                    scenesToUnload.Add(scene.name);
                }
            }

            // Unload scenes
            foreach (var sceneNameToUnload in scenesToUnload)
            {
                if (sceneHolder.ContainsKey(sceneNameToUnload))
                {
                    await Addressables.UnloadSceneAsync(sceneHolder[sceneNameToUnload]);
                    sceneHolder.Remove(sceneNameToUnload);
                }
                else
                {
                    var scene = SceneManager.GetSceneByName(sceneNameToUnload);
                    if (scene.IsValid())
                    {
                        await SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            // Load new scene
            var handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            while (!handle.IsDone)
            {
                onProgress?.Invoke(handle.PercentComplete);
                await UniTask.Yield();
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                string loadedSceneName = handle.Result.Scene.name;
                if (!sceneHolder.ContainsKey(loadedSceneName))
                {
                    sceneHolder.Add(loadedSceneName, handle);
                }
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadedSceneName));
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