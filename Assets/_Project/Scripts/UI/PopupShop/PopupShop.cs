using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class PopupShop : Popup
{
    [ReadOnly] public ShopState currentShopState = ShopState.Skin;
    public Transform content;
    public GameObject btnOnShopSkin;
    public GameObject btnOffShopSkin;
    public GameObject btnOnShopGun;
    public GameObject btnOffShopGun;
    [SerializeField] private ShopItem shopItemPrefabs;
    [SerializeField] private PlayerSkinController playerSkinController;
    private ItemConfig itemConfig;
    private List<ItemData> listItemDatas = new List<ItemData>();
    protected override void BeforeShow()
    {
        base.BeforeShow();
        itemConfig = ConfigController.ItemConfig;
        AdsManager.HideBanner();
        PopupController.Instance.Show<PopupUI>();
        SetupState(currentShopState);
    }

    private void SetupDefaultBtn()
    {
        btnOffShopGun.SetActive(false);
        btnOffShopSkin.SetActive(false);
        btnOnShopGun.SetActive(false);
        btnOnShopSkin.SetActive(false);
    }

    private void SetupBtn(ShopState _shopState)
    {
        SetupDefaultBtn();
        switch (_shopState)
        {
            case ShopState.Skin:
                btnOnShopSkin.SetActive(true);
                btnOffShopGun.SetActive(true);
                break;
            case ShopState.Gun:
                btnOffShopSkin.SetActive(true);
                btnOnShopGun.SetActive(true);
                break;
        }
    }

    public void SetupState(ShopState _shopState)
    {
        Utility.Clear(content);
        currentShopState = _shopState;
        SetupBtn(_shopState);
        switch (_shopState)
        {
            case ShopState.Skin:
                listItemDatas = itemConfig.ListSkinDatas;
                break;
            case ShopState.Gun:
                listItemDatas = itemConfig.ListGunDatas;
                break;
        }

        for (int i = 0; i < listItemDatas.Count; i++)
        {
            ShopItem shopItem = Instantiate(shopItemPrefabs, content);
            shopItem.InitItemData(listItemDatas[i]);
        }
    }

    public void OnClickSkinShop()
    {
        if (currentShopState != ShopState.Skin)
        {
            currentShopState = ShopState.Skin;
            SetupState(currentShopState);
        }
    }

    public void OnClickGunShop()
    {
        if (currentShopState != ShopState.Gun)
        {
            currentShopState = ShopState.Gun;
            SetupState(currentShopState);
        }
    }
    public void ViewSkin(int idSkin)
    {
        playerSkinController.ViewSkin(idSkin);
    }
}

public enum ShopState
{
    Skin,
    Gun,
}
