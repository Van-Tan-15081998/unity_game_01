using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_AbilityState_Boss : VT_EnemyState
{
    private VT_Enemy_Boss enemy;

    public VT_AbilityState_Boss(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.flameThrowDuration;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.SetAbilityOnCooldown();

        enemy.bossVisuals.ResetBatteries();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (stateTimer < 0 && enemy.flamethrowActive)
        {
            enemy.ActivateFlameThrower(false);
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        enemy.ActivateFlameThrower(true);

        enemy.bossVisuals.DischargeBatteries();
    }
}
