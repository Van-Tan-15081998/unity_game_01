using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_PlayerAnimationEvents : MonoBehaviour
{
    private VT_PlayerWeaponVisuals visualController;
    private VT_PlayerWeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<VT_PlayerWeaponVisuals>();
        weaponController = GetComponentInParent<VT_PlayerWeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();
    }

    public void ReturnRig()
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandWeight();
    }

    public void WeaponGrapIsOver()
    {
        visualController.SetBusyEquipingWeaponTo(false);
    }

    public void SwitchOnWeaponModel()
    {
        visualController.SwitchOnCurrentWeaponModel();
    }
}
