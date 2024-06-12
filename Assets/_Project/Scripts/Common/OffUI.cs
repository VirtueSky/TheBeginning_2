using System.Collections.Generic;
using System.Linq;
using Base.Data;
using Base.Global;
using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Inspector;
using VirtueSky.Misc;

public class OffUI : MonoBehaviour
{
    [SerializeField] private List<Graphic> listGraphics;

    private void OnEnable()
    {
        GetComponentUI();
        Observer.OffUIChanged += Setup;
        Setup(UserData.IsOffUIDebug);
    }

    private void OnDisable()
    {
        Observer.OffUIChanged -= Setup;
    }

    void Setup(bool isOff)
    {
        if (listGraphics.Count == 0) return;
        foreach (var graphic in listGraphics)
        {
            graphic.SetAlpha(isOff ? 0 : 1);
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