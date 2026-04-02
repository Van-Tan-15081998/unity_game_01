using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_PlayerHitbox : VT_HitBox
{
    private VT_Player player;
    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInParent<VT_Player>();
    }

    public override void TakeDamage(int damage)
    {
        player.health.ReduceHealth(damage);
    }
}
