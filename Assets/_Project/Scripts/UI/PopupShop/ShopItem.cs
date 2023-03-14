using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [ReadOnly] public StateItem stateItem;
    public GameObject bgUnSelect;
    public GameObject bgSelect;
    public Image icon;
    public GameObject btnBuy;
    public GameObject btnCannotBuy;
    public GameObject btnAds;
    public GameObject btnDaily;
    public TextMeshProUGUI textCoin;
    public TextMeshProUGUI textCoinCannotBuy;

    private ItemData itemData;
    public PopupShop PopupShop => PopupController.Instance.Get<PopupShop>() as PopupShop;
    public void InitItemData(ItemData _itemData)
    {
        itemData = _itemData;
        SetupStateItem();
        SetupUI();
    }

    private void SetupDefaultUI()
    {
        bgSelect.SetActive(false);
        bgUnSelect.SetActive(false);
        btnBuy.SetActive(false);
        btnCannotBuy.SetActive(false);
        btnAds.SetActive(false);
        btnDaily.SetActive(false);
        icon.sprite = itemData.imageIcon;
        icon.SetNativeSize();
        textCoin.text = textCoinCannotBuy.text = itemData.Coin.ToString();
    }

    private void SetupStateItem()
    {
        if (itemData.typeItem == TypeItem.Skin && itemData.id == Data.CurrentIdSkin)
        {
            stateItem = StateItem.Select;
        }
        else if (itemData.typeItem == TypeItem.Gun && itemData.id == Data.CurrentIdGun)
        {
            stateItem = StateItem.Select;
        }
        else
        {
            stateItem = StateItem.UnSelect;
        }
    }

    private void SetupUI()
    {
        SetupDefaultUI();
        
        if (itemData.IsUnlock)
        {
            if (stateItem == StateItem.Select)
            {
                bgSelect.SetActive(true);
            }
            else
            {
                bgUnSelect.SetActive(true);
            }
        }
        else
        {
            bgUnSelect.SetActive(true);
            switch (itemData.typeBuy)
            {
                case TypeBuy.Default:
                    break;
                case TypeBuy.Coin:
                    if (CanBuyItem())
                    {
                        btnBuy.SetActive(true);
                    }
                    else
                    {
                        btnCannotBuy.SetActive(true);
                    }
                    break;
                case TypeBuy.Ads:
                    btnAds.SetActive(true);
                    break;
                case TypeBuy.DailyReward:
                    btnDaily.SetActive(true);
                    break;
            }
        }
    }

    private bool CanBuyItem()
    {
        if (Data.CurrencyTotal >= itemData.Coin && !itemData.IsUnlock) return true;
        return false;
    }

    public void OnClickSelect()
    {
        if (itemData.IsUnlock && stateItem == StateItem.UnSelect)
        {
            switch (itemData.typeItem)
            {
                case TypeItem.Skin:
                    stateItem = StateItem.Select;
                    Data.CurrentIdSkin = itemData.id;
                    PopupShop.SetupState(PopupShop.currentShopState);
                    break;
                case TypeItem.Gun:
                    stateItem = StateItem.Select;
                    Data.CurrentIdGun = itemData.id;
                    PopupShop.SetupState(PopupShop.currentShopState);
                    break;
            }
        }


        PopupShop.ViewSkin(itemData.id);
    }

    public void OnClickBuy()
    {
        Data.CurrencyTotal -= itemData.Coin;
        itemData.IsUnlock = true;
      //  SoundController.Instance.PlayFX(SoundType.CompletePurchase);
        SetupUI();
        OnClickSelect();
    }

    public void OnClickDaily()
    {
        PopupShop.Hide();
        PopupController.Instance.Show<PopupDailyReward>();
    }

    public void OnClickWatchAds()
    {
        if (Data.IsTesting)
        {
            itemData.IsUnlock = true;
            //SoundController.Instance.PlayFX(SoundType.CompletePurchase);
            SetupUI();
            OnClickSelect();
        }
        else
        {
            AdsManager.ShowRewardAds((() =>
            {
                itemData.IsUnlock = true;
                //SoundController.Instance.PlayFX(SoundType.CompletePurchase);
                SetupUI();
                OnClickSelect();
            }), () => { itemData.IsUnlock = false; });
        }
    }
}

public enum StateItem
{
    Select,
    UnSelect
}
