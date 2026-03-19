using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType { smallBox, bigBox };

[System.Serializable]
public struct AmmoData
{
    public VT_WeaponType weaponType;

    [Range(10, 100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
}

public class VT_PickupAmmo : VT_Interactable
{


    [SerializeField] private AmmoBoxType boxType;



    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        SetupBoxModel();
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false);

            if (i == ((int)boxType))
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }

        }
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if (boxType == AmmoBoxType.bigBox)
        {
            currentAmmoList = bigBoxAmmo;
        }

        foreach (AmmoData ammo in currentAmmoList)
        {
            VT_Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);

            AddBulletsToWeapon(weapon, GetBulletAmount(ammo));
        }

        VT_ObjectPool.instance.ReturnObject(gameObject);
    }

    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmount = Random.Range(min, max);

        return Mathf.RoundToInt(randomAmmoAmount);
    }

    private void AddBulletsToWeapon(VT_Weapon weapon, int amount)
    {
        if (weapon == null)
        {
            return;
        }

        weapon.totalReserveAmmo += amount;
    }
}
