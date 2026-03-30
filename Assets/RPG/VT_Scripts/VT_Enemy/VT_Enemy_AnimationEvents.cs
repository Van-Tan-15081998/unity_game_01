using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_AnimationEvents : MonoBehaviour
{
    private VT_Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<VT_Enemy>();
    }

    public void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    public void StartManualMovement()
    {
        enemy.ActiveManualMovement(true);
    }
    public void StopManualMovement()
    {
        enemy.ActiveManualMovement(false);
    }

    public void StartManualRotation()
    {
        enemy.ActiveManualRotation(true);
    }
    public void StopManualRotation()
    {
        enemy.ActiveManualRotation(false);
    }

    public void AbilityEvent()
    {
        enemy.AbilityTrigger();  
    }

    public void EnableIK()
    {
        enemy.visuals.EnableIK(true, true, 1.0f);
    }

    public void EnableWeaponModel()
    {
        enemy.visuals.EnableWeaponModel(true);
        enemy.visuals.EnableSecondaryWeaponModel(false);
    }
}
