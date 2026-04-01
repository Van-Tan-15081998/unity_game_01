using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_Hitbox : VT_HitBox
{

    private VT_Enemy enemy;
    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<VT_Enemy>();   
    }

    public override void TakeDamage()
    {
        base.TakeDamage();

        enemy.GetHit();
    }

}
