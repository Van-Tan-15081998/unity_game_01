using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_ItemPickup : MonoBehaviour
{
    [SerializeField] private VT_Weapon weapon;

    // Hàm này sẽ được gọi khi có một collider khác đi vào trigger collider của đối tượng này
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<VT_PlayerWeaponController>()?.PickupWeapon(weapon);  
    }
}
