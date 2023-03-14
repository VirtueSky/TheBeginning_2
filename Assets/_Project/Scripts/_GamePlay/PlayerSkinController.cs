using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSkinController : MonoBehaviour
{
    public SkinnedMeshRenderer SkinBase;
    private List<SkinPlayer> listPlayerSkin => GetComponentsInChildren<SkinPlayer>(true).ToList();

    private void Start()
    {
        SetupSkin();
        Observer.CurrentSkinChanged += SetupSkin;
    }

    private void OnDestroy()
    {
        Observer.CurrentSkinChanged -= SetupSkin;
    }

    public void ViewSkin(int idSkin)
    {
        SkinBase.material = ConfigController.ItemConfig.GetSkinDataById(idSkin).matSkin;
        foreach (var VARIABLE in listPlayerSkin)
        {
            if (VARIABLE.id == idSkin)
            {
                VARIABLE.gameObject.SetActive(true);
            }
            else
            {
                VARIABLE.gameObject.SetActive(false);
            }
        }
    }

    public void SetupSkin()
    {
        SkinBase.material = ConfigController.ItemConfig.GetSkinDataById(Data.CurrentIdSkin).matSkin;
        foreach (var VARIABLE in listPlayerSkin)
        {
            if (VARIABLE.id == Data.CurrentIdSkin)
            {
                VARIABLE.gameObject.SetActive(true);
            }
            else
            {
                VARIABLE.gameObject.SetActive(false);
            }
        }
    }
}
