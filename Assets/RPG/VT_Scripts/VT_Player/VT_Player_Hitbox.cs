using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Player_Hitbox : VT_HitBox
{
    private VT_Player player;
    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInParent<VT_Player>();
    }

    public override void TakeDamage(int damage)
    {
        int newDamage = Mathf.RoundToInt(damage * damageMultiplier);

        Debug.LogWarning("Sat thuong: " + newDamage);

        player.health.ReduceHealth(newDamage);
    }
}
