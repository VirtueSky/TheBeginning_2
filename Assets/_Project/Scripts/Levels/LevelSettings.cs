using System.Collections.Generic;
using UnityEngine;
using VirtueSky.Inspector;
using VirtueSky.Linq;

namespace Base.Levels
{
    [EditorIcon("icon_scriptable"), HideMonoScript]
    public class LevelSettings : ScriptableObject
    {
        [SerializeField] private int maxLevel;
        [SerializeField] private int startLoopLevel;

        [SerializeField] private string pathLoad = "Assets/_Project/Prefabs/Levels";
        [SerializeField] private List<Level> listLevels;


        public int MaxLevel => maxLevel;
        public int StartLoopLevel => startLoopLevel;
        public List<Level> ListLevels => listLevels;

        public Level GePrefabLevel(string levelName)
        {
            return listLevels.FirstOrDefault(item => item.name == levelName);
        }

#if UNITY_EDITOR
        [Button]
        void LoadPrefabLevel()
        {
            listLevels.Clear();
            var levels = VirtueSky.UtilsEditor.FileExtension.GetPrefabsFromFolder<Level>(pathLoad);
            foreach (var t in levels)
            {
                if (t != null)
                {
                    listLevels.Add(t);
                }
            }
        }

#endif
    }
}