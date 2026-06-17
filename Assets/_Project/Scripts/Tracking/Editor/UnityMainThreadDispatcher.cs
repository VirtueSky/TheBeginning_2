using UnityEditor;
using System;
using System.Collections.Generic;

namespace VirtueSky.Tracking.Editor
{
    /// <summary>
    /// Utility to run actions on Unity main thread from background threads
    /// </summary>
    public class UnityMainThreadDispatcher
    {
        private static UnityMainThreadDispatcher instance;
        private readonly Queue<Action> executionQueue = new Queue<Action>();

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnityMainThreadDispatcher();
                    EditorApplication.update += instance.Update;
                }

                return instance;
            }
        }

        public void Enqueue(Action action)
        {
            lock (executionQueue)
            {
                executionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (executionQueue)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}