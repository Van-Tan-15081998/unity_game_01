using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_AttackState_Boss : VT_EnemyState
{
    private VT_Enemy_Boss enemy;

    public float lastTimeAttacked {  get; private set; }    

    public VT_AttackState_Boss(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetFloat("VT_AttackAnimIndex", Random.Range(0, 2)); /// Có 2 attack chỉ số từ 0->1

        enemy.agent.isStopped = true;


        stateTimer = 1f;
        
    }

    public override void Exit()
    {
        base.Exit();

        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            enemy.FaceTarget(enemy.player.position, 20);
        }

        if (triggerCalled)
        {
            /// Nếu Player vẫn đứng yên tại chỗ thì Boss không cần phải di chuyển tới vị trí của Player nữa
            if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.idleState);
            } else
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }
}
