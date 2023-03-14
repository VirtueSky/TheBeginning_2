using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class LoadConfig
{
    public static T GetConfig<T>(string path) where T : ScriptableObject
    {
        var fileEntries = Directory.GetFiles(path, ".", SearchOption.AllDirectories);

        foreach (var fileEntry in fileEntries)
            if (fileEntry.EndsWith(".asset"))
            {
                var item =
                    AssetDatabase.LoadAssetAtPath<T>(fileEntry.Replace("\\", "/"));
                if (item)
                    return item;
            }

        return null;
    }

    public static List<T> GetConfigs<T>(string path) where T : ScriptableObject
    {
        var fileEntries = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
        var result = new List<T>();
        foreach (var fileEntry in fileEntries)
            if (fileEntry.EndsWith(".asset"))
            {
                var item = AssetDatabase.LoadAssetAtPath<T>(fileEntry.Replace("\\", "/"));

                if (item)
                    result.Add(item);
            }

        if (result.Count > 0)
            return result;

        return null;
    }
}
