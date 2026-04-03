using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_HitBox : VT_HitBox
{

    private VT_Enemy enemy;
    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<VT_Enemy>();   
    }

    public override void TakeDamage(int damage)
    {
        int newDamage = Mathf.RoundToInt(damage * damageMultiplier);

        enemy.GetHit(newDamage);
    }

}
