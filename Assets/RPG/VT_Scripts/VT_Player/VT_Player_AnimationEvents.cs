using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Player_AnimationEvents : MonoBehaviour
{
    private VT_Player_WeaponVisuals visualController;
    private VT_Player_WeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<VT_Player_WeaponVisuals>();
        weaponController = GetComponentInParent<VT_Player_WeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();

        /// Đặt trạng thái vũ khí sẵn sàng sau khi tái nạp hoàn tất để cho phép người chơi bắn 
        /// hoặc thực hiện các hành động khác với vũ khí.
        weaponController.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandWeight();
    }

    public void WeaponEquipingIsOver()
    {
        /// Đặt trạng thái vũ khí sẵn sàng sau khi trang bị hoàn tất để cho phép người chơi bắn
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel()
    {
        visualController.SwitchOnCurrentWeaponModel();
    }
}
