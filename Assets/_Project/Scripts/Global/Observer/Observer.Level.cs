using System;
using Base.Levels;

namespace Base.Global
{
    public partial struct Observer
    {
        public static Action<Level> OnStartLevel;
        public static Action<Level> OnWinLevel;
        public static Action<Level> OnLoseLevel;
        public static Action<Level> OnNextLevel;
        public static Action<Level> OnReplayLevel;
    }
}