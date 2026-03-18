using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "VT_Weapon_Data_", menuName = "[VT] Weapon System/[VT] Weapon Data")]
public class VT_WeaponData : ScriptableObject
{

    public string weaponName;

    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;


    [Header("Regular shot")]
    public VT_ShootType shootType;
    public int bulletsPerShot = 1;
    public float fireRate;


    [Header("Burst shot")]
    public bool burstAvalible;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = .1f;

    [Header("Weapon spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = .15f;

    [Header("Weapon generics")]
    public VT_WeaponType weaponType;
    [Range(1, 3)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipmentSpeed = 1;
    [Range(4, 8)]
    public float gunDistance = 4;
    [Range(4, 8)]
    public float cameraDistance = 6;
}
