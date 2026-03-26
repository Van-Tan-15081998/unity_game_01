using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "VT_Weapon_Data_", menuName = "[VT] Enemy Data/[VT] Melee Weapon Data")]

public class VT_Enemy_MeleeWeaponData : ScriptableObject
{
    public List<VT_AttackData_EnemyMelee> attackData;

    public float turnSpeed = 10;
}
