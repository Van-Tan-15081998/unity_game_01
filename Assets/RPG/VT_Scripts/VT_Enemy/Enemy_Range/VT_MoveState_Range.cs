using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_MoveState_Range : VT_EnemyState
{
    private VT_Enemy_Range enemy;
    private Vector3 destination;

    public VT_MoveState_Range(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.walkSpeed;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }


}