using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_EnemyShield : MonoBehaviour
{
    private VT_Enemy_Melee enemy;

    [SerializeField] private int durability;

    private void Awake()
    {
        enemy = GetComponentInParent<VT_Enemy_Melee>();
    }

    public void ReduceDurability()
    {
        durability--;

        /// Khiên vỡ => Quay lại ChaseState
        if (durability <= 0 )
        {
            enemy.anim.SetFloat("VT_ChaseIndex", 0);
            Destroy(gameObject);
        }
    }
}
