using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Base.Global
{
    public struct Static
    {
        public static Dictionary<string, Scene> sceneHolder =
            new Dictionary<string, Scene>();
    }
}