using System;
using System.Collections.Generic;
using System.Data.Common;
using Pancake;
using Pancake.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "ScriptableObject/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    public List<ItemData> ListSkinDatas;
    public List<ItemData> ListGunDatas ;

    public void InitItem()
    {
        UnlockItemDefaul();
    }

    public void UnlockItemDefaul()
    {
        ListSkinDatas.First(dt => dt.typeBuy == TypeBuy.Default).IsUnlock = true;
        ListGunDatas.First(dt => dt.typeBuy == TypeBuy.Default).IsUnlock = true;
    }

    #region Skin

    public ItemData GetSkinDataById(int _id)
    {
        return ListSkinDatas.First(dt => dt.id == _id);
    }
    #endregion

    #region Gun

    public ItemData GetGunDataById(int _id)
    {
        return ListGunDatas.First(dt => dt.id == _id);
    }

    #endregion
}

[Serializable]
public class ItemData
{
    public TypeItem typeItem;
    public int id;
    [ShowIf("typeItem", TypeItem.Skin)] 
    public SkinName skinName;

    [ShowIf("typeItem", TypeItem.Gun)] 
    public GunName gunName;
    
    public Sprite imageIcon;
    public TypeBuy typeBuy;
    [ShowIf("typeItem", TypeItem.Skin)] public Material matSkin;
    [ShowIf("typeBuy", global::TypeBuy.Coin)]
    public int Coin;

    public bool IsUnlock
    {
        get
        {
            Data.KeyItemCheckUnlocked = typeItem + "_" + id;
            return Data.IsItemUnlocked;
        }
        set
        {
            Data.KeyItemCheckUnlocked = typeItem + "_" + id;
            Data.IsItemUnlocked = value;
        }
    }
}

public enum TypeItem
{
    Skin,
    Gun,
}

public enum TypeBuy
{
    Default,
    Coin,
    Ads,
    DailyReward,
}

public enum SkinName
{
    SkinDefault, 
    Cowboy,
    DeathStroke,
    Ghost,
    GunpowderSoldier,
    Hunter,
    Knight,
    Military,
    Musketeer,
    Rambo,
    RobinHood,
    Terrorist,
}

public enum GunName
{
    GunDefault,
    Gun1,
    Gun2,
    Gun3,
    Gun4,
    Gun5,
    Gun6,
    Gun7,
    Gun8,
    Gun9,
}