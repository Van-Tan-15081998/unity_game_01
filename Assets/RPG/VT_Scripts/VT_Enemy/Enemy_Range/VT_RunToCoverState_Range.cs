using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VT_RunToCoverState_Range : VT_EnemyState
{
    private VT_Enemy_Range enemy;
    private Vector3 destination;

    public float lastTimeTookCover {  get; private set; }   

    public VT_RunToCoverState_Range(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        destination = enemy.currentCover.transform.position;

        enemy.visuals.EnableIK(true, false);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();

        lastTimeTookCover = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(enemy.transform.position, destination) < .5f)
        {
            stateMachine.ChangeState(enemy.battleState);
        } 
        else
        {
            //Debug.LogWarning("Không thể ẩn nấp!" + Vector3.Distance(enemy.transform.position, destination).ToString());
        }
    }
}
