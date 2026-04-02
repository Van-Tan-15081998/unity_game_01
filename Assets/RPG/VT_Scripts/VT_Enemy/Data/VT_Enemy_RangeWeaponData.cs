using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VT_Weapon_Data_", menuName = "[VT] Enemy Data/[VT] Range Weapon Data")]

public class VT_Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public VT_Enemy_RangeWeaponType weaponType;
    public float fireRate = 1f; /// Bullets Per Second

    public int minBulletsPerAttack = 1;
    public int maxBulletsPerAttack = 1;

    public float minWeaponCooldown = 2;
    public float maxWeaponCooldown = 3;

    [Header("Bullet Details")]
    public int bulletDamage;
    [Space]
    public float bulletSpeed = 20;
    public float weaponSpread = .1f;

    public int GetBulletsPerAttack()
    {
        return Random.Range(minBulletsPerAttack, maxBulletsPerAttack + 1);  
    }

    public float GetWeaponCooldown()
    {
        return Random.Range(minWeaponCooldown, maxWeaponCooldown);
    }

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        /// Tạo một giá trị ngẫu nhiên trong khoảng từ -spreadAmount đến +spreadAmount.
        float randomizedValue = Random.Range(-weaponSpread, weaponSpread);

        /// Tạo một Quaternion đại diện cho sự xoay ngẫu nhiên dựa trên giá trị ngẫu nhiên đã tạo.
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue / 2, randomizedValue);

        /// Áp dụng sự xoay ngẫu nhiên vào hướng ban đầu của viên đạn để tạo ra hiệu ứng giật.
        return spreadRotation * originalDirection;
    }
}
