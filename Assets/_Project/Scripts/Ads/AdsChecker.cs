using System;
using Pancake;
using Pancake.Monetization;
using UnityEngine;
using UnityEngine.UI;

public class AdsChecker : MonoBehaviour
{
    public Sprite haveAds;
    public Sprite noAds;
    public bool useLoadingIcon;
    [ShowIf("useLoadingIcon")] public Image loadingIcon;

    private Image Image => GetComponent<Image>();
    private ButtonCustom Button => GetComponent<ButtonCustom>();

    private void Update()
    {
        if (Advertising.IsRewardedAdReady())
        {
            Image.sprite = haveAds;
            Button.CanClick = true;
            if (useLoadingIcon && loadingIcon) loadingIcon.gameObject.SetActive(false);
        }
        else
        {
            if (Data.IsTesting)
            {
                Image.sprite = haveAds;
                Button.CanClick = true;
                if (useLoadingIcon && loadingIcon) loadingIcon.gameObject.SetActive(false);
            }
            else
            {
                Image.sprite = noAds;
                Button.CanClick = false;
                if (useLoadingIcon && loadingIcon && !Data.IsTesting)
                {
                    loadingIcon.gameObject.SetActive(true);
                }
            }
        }
    }
}