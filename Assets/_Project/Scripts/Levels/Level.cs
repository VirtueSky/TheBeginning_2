using UnityEngine;
using VirtueSky.Core;

namespace Base.Levels
{
    public class Level : BaseMono
    {
        public Transform CacheTransform { get; set; }

        private void Awake()
        {
            if (CacheTransform == null)
            {
                CacheTransform = transform;
            }
        }
    }
}