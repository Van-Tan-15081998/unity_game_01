using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_Shield : MonoBehaviour, VT_IDamagable
{
    private VT_Enemy_Melee enemy;

    [SerializeField] private int durability;

    private void Awake()
    {
        enemy = GetComponentInParent<VT_Enemy_Melee>();

        durability = enemy.shieldDurability;
    }

    public void ReduceDurability(int damage)
    {
        durability -= damage;

        /// Khiên vỡ => Quay lại ChaseState
        if (durability <= 0 )
        {
            enemy.anim.SetFloat("VT_ChaseIndex", 0);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        ReduceDurability(damage);
    }
}
