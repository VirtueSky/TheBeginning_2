using System;
using Base.Game;
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

            GameManager.OnWinLevel += OnWinLevel;
            GameManager.OnLoseLevel += OnLoseLevel;
        }

        private void OnDestroy()
        {
            GameManager.OnWinLevel -= OnWinLevel;
            GameManager.OnLoseLevel -= OnLoseLevel;
        }

        private void OnWinLevel(Level level)
        {
        }

        private void OnLoseLevel(Level level)
        {
        }
    }
}