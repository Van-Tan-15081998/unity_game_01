using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_MoveState_Melee : VT_EnemyState
{
    private VT_Enemy_Melee enemey;
    private Vector3 destination;

    public VT_MoveState_Melee(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemey = enemyBase as VT_Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        destination = enemey.GetPatrolDestination();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemey.agent.SetDestination(destination);

        if (enemey.agent.remainingDistance <= 1)
        {
            stateMachine.ChangeState(enemey.idleState);
        }
    }
}
