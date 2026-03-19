using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType { SideEquipAnimation, BackEquipAnimation }

public enum HoldType { CommonHold = 1, LowHold, HighHold };
// Enum CommonHold có value là 1 để tương ứng với thứ tự trong Animator.


public class VT_WeaponModel : MonoBehaviour
{
    public VT_WeaponType weaponType;
    public EquipType equipAnimationType;
    public HoldType holdType;


    public Transform gunPoint;
    public Transform holdPoint;
}
