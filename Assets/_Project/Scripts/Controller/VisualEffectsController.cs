using System;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class VisualEffectsController : SingletonDontDestroy<VisualEffectsController>
{
    public List<VisualEffectData> visualEffectDatas;

    public VisualEffectData GetVisualEffectDataByType(VisualEffectType visualEffectType)
    {
        return visualEffectDatas.Find(item => item.visualEffectType == visualEffectType);
    }

    public void SpawnEffect(VisualEffectType visualEffectType,Vector3 position, Transform parent, Vector3? localScale = null, bool isDestroyedOnEnd = true, float timeDestroy = 3f)
    {
        VisualEffectData visualEffectData = GetVisualEffectDataByType(visualEffectType);
        if (visualEffectData != null)
        {
            GameObject randomEffect = visualEffectData.GetRandomEffect();
            GameObject effect = Instantiate(randomEffect, parent, false);
            effect.transform.position = position;
            if (localScale != null) effect.transform.localScale = localScale.Value;
            if (isDestroyedOnEnd) Destroy(effect, timeDestroy);
        }
    }
    
    private bool IsItemExistedByVisualEffectType(VisualEffectType visualEffectType)
    {
        foreach (VisualEffectData item in visualEffectDatas)
        {
            if (item.visualEffectType == visualEffectType)
            {
                return true;
            }
        }

        return false;
    }

    [Button]
    public void UpdateVisualEffects()
    {
        for (int i = 0; i < Enum.GetNames(typeof(VisualEffectType)).Length; i++)
        {
            VisualEffectData visualEffectData = new VisualEffectData();
            visualEffectData.visualEffectType = (VisualEffectType) i;
            if (IsItemExistedByVisualEffectType(visualEffectData.visualEffectType)) continue;
            visualEffectDatas.Add(visualEffectData);
        }

        visualEffectDatas = visualEffectDatas.GroupBy(elem => elem.visualEffectType).Select(group => group.First()).ToList();
    }
}

[Serializable]
public class VisualEffectData
{
    public List<GameObject> effectList;
    public VisualEffectType visualEffectType;

    public GameObject GetRandomEffect()
    {
        return effectList[Random.Range(0, effectList.Count)];
    }
}

public enum VisualEffectType
{
    Default,
}
