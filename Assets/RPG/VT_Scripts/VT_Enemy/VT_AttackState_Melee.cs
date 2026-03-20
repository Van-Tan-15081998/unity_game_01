using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_AttackState_Melee : VT_EnemyState
{
    private VT_Enemy_Melee enemy;
    private Vector3 attackDirection;

    private const float MAX_ATTACK_DISTANCE = 50f;

    public VT_AttackState_Melee(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.PullWeapon();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;    

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position =
            Vector3.MoveTowards(
                enemy.transform.position,
                attackDirection,
                enemy.attackMoveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
