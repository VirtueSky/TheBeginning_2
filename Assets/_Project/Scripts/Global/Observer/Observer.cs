using System;
using Base.Game;
using UnityEngine;

namespace Base.Global
{
    public partial struct Observer
    {
        // Debug
        public static Action IsTestingChanged;


        // Level Spawn
        public static Action CurrentLevelChanged;

        public static Action<GameState> OnChangeStateGame;

        public static Action OnClaimDailyReward;

        public static Action<bool> OffUIChanged;
    }
}