using UnityEngine;

public class VT_PickupWeapon : VT_Interactable
{
    
    [SerializeField] private VT_WeaponData weaponData;
    [SerializeField] private VT_Weapon weapon;

    [SerializeField] private VT_BackupWeaponModel[] models;

    private bool oldWeapon;

    private void Start()
    {
        if (oldWeapon == false)
        {
            weapon = new VT_Weapon(weaponData);
        }

        SetupGameObject();
    }

    public void SetupPickupWeapon(VT_Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon;
        weaponData = weapon.weaponData;

        this.transform.position = transform.position + new Vector3(0, .75f, 0);
    }

    [ContextMenu("[VT] Update Item Model")] /// => Click chuột phải vào Script trong Inspector
    /// => Chọn -[VT] Update Item Model- => Lập tức Giao diện được cập nhật dựa theo data đã cung cấp
    public void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();
        
        SetupWeaponModel();
    }

    public void SetupWeaponModel()
    {
        foreach(VT_BackupWeaponModel model in models) {
            model.gameObject.SetActive(false);

            if (model.weaponType == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(newMesh: model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickupWeapon(weapon);

        VT_ObjectPool.instance.ReturnObject(gameObject);
    }


}
