using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_PlayerHealth : VT_HealthController
{
    private VT_Player player;

    public bool isDead { get; private set;}

    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInParent<VT_Player>();
    }

    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);

        if (ShouldDie())
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        player.anim.enabled = false;
        player.ragdoll.RagdollActive(true);
    }
}
