using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VT_MoveState_Melee : VT_EnemyState
{
<<<<<<< HEAD
    private VT_Enemy_Melee enemey;
=======
    private VT_Enemy_Melee enemy;
>>>>>>> main
    private Vector3 destination;

    public VT_MoveState_Melee(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

<<<<<<< HEAD
        destination = enemey.GetPatrolDestination();
=======
        enemy.agent.speed = enemy.moveSpeed;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
>>>>>>> main
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

<<<<<<< HEAD
        enemey.agent.SetDestination(destination);

        if (enemey.agent.remainingDistance <= 1)
=======
        if (enemy.PlayerInAggresionRange())
>>>>>>> main
        {
            stateMachine.ChangeState(enemy.recoveryState);
            return;
        }

        enemy.transform.rotation = enemy.FaceTarget(GetNextPathPoint());

        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }


}
