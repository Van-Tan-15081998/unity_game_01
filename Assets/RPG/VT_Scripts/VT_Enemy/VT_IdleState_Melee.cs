using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_IdleState_Melee : VT_EnemyState
{
    private VT_Enemy_Melee enemey;

    public VT_IdleState_Melee(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemey = enemyBase as VT_Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemyBase.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            // Change State to move
            stateMachine.ChangeState(enemey.moveState);
        }
    }
}
