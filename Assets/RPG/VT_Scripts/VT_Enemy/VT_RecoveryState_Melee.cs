using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_RecoveryState_Melee : VT_EnemyState
{
    private VT_Enemy_Melee enemy;

    public VT_RecoveryState_Melee(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.FaceTarget(enemy.player.position);

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
