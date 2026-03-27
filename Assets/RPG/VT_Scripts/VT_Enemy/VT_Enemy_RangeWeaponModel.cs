using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VT_Enemy_RangeWeaponHoldType { Common, LowHold, HighHold };

public class VT_Enemy_RangeWeaponModel : MonoBehaviour
{
    public Transform gunPoint;

    [Space]
    public VT_Enemy_RangeWeaponType weaponType;
    public VT_Enemy_RangeWeaponHoldType weaponHoldType;

    public Transform leftHandTarget;
    public Transform leftElbowTarget;


}
