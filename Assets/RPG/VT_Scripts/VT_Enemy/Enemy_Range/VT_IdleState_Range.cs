using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_IdleState_Range : VT_EnemyState
{
    private VT_Enemy_Range enemy;

    public VT_IdleState_Range(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetFloat("VT_IdleAnimIndex", Random.Range(0, 3)); /// Có 3 anim chỉ số từ 0->2
        Debug.LogWarning("VT_IdleAnimIndex: " + enemy.anim.GetFloat("VT_IdleAnimIndex"));

        enemy.visuals.EnableIK(true, false);

        if (enemy.weaponType == VT_Enemy_RangeWeaponType.Pistol)
        {
            enemy.visuals.EnableIK(false, false);
        }

        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
