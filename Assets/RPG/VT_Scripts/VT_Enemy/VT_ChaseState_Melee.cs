using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_ChaseState_Melee :  VT_EnemyState
{
    private VT_Enemy_Melee enemy;
    private float lastTimeUpdateDestination;

    public VT_ChaseState_Melee(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.chaseSpeed;

        enemy.agent.isStopped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        enemy.transform.rotation = enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }

    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdateDestination + .25f)
        {
            lastTimeUpdateDestination = Time.time; 

            return true;
        }

        return false;   
    }
}
