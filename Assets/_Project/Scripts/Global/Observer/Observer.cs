using System;
using UnityEngine;

namespace Base.Global
{
    public partial struct Observer
    {
        #region GameSystem

        // Debug
        public static Action IsTestingChanged;

        // Currency
        public static Action SaveCoinTotal;

        public static Action CoinTotalChanged;

        // Level Spawn
        public static Action CurrentLevelChanged;

        #endregion

        public static Action<Vector3> SetPositionCoinGenerate;
    }
}