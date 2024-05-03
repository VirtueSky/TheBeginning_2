using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Base.Global
{
    public struct Static
    {
        public static Dictionary<string, AsyncOperationHandle<SceneInstance>> sceneHolder =
            new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
    }
}