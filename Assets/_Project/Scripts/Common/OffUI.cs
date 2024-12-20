using System.Collections.Generic;
using System.Linq;
using Base.Data;
using Base.Global;
using UnityEngine;
using UnityEngine.UI;
using Virtuesky.Events;
using VirtueSky.Inspector;
using VirtueSky.Misc;

public class OffUI : MonoBehaviour
{
    [SerializeField] private List<Graphic> listGraphics = new List<Graphic>();

    private void OnEnable()
    {
        GetComponentUI();
        EventName.OnOffUIChanged.AddListener(Setup);
        Setup(UserData.IsOnOffUIDebug);
    }

    private void OnDisable()
    {
        EventName.OnOffUIChanged.RemoveListener(Setup);
    }

    void Setup(bool isOff)
    {
        if (listGraphics.Count == 0) return;
        foreach (var graphic in listGraphics)
        {
            graphic.SetAlpha(isOff ? 1 : 0);
        }
    }

    [Button]
    void GetComponentUI()
    {
        List<Graphic> listTemp = GetComponentsInChildren<Graphic>(true).ToList();
        listGraphics.Clear();
        foreach (var graphic in listTemp)
        {
            if (graphic.color.a != 0)
            {
                listGraphics.Add(graphic);
            }
        }
    }
#if UNITY_EDITOR
    private void Reset()
    {
        GetComponentUI();
    }
#endif
}