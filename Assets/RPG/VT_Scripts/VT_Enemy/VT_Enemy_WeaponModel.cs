using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_WeaponModel : MonoBehaviour
{
    public VT_Enemy_MeleeWeaponType weaponType;

    public AnimatorOverrideController overrideController;

    public VT_Enemy_MeleeWeaponData weaponData;

    [SerializeField] private GameObject[] trailEffects;

    [Header("Damage Attributes")]
    public Transform[] damagePoints;
    public float attackRadius;

    private void Awake()
    {
        //EnableTrailEffect(false); /// Không cần nếu Inspector đã Set Unactive
    }

    public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }

    /// <summary>
    /// Trong Isnpector của object vũ khí, click chuột phải vào script VT_Enemy_WeaponModel
    /// => Chọn "Assign Damage Point Transforms"
    /// => Lập tức damagePoints sẽ được ASSIGN từ các trailEffects
    /// </summary>
    [ContextMenu("Assign Damage Point Transforms")]
    private void GetDamagePoints()
    {
        /// Có thể dựa vào trailEffects để áp dụng vào điểm/vùng gây ra sát thương
        /// 
        damagePoints = new Transform[trailEffects.Length];

        for (int i = 0; i < trailEffects.Length; i++)
        {
            damagePoints[i] = trailEffects[i].transform;
        }
    }

    private void OnDrawGizmos()
    {
        /// Vẽ phạm vi vùng gây sát thương của vũ khí
        /// Lấy vị trí của điểm thuộc damagePoints làm tâm tọa độ và bán kính sát thương
        if (damagePoints.Length > 0)
        {
            foreach (Transform point in damagePoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(point.position, attackRadius);
            }
        }

    }
}
