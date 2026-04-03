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

        enemy.bossVisuals.EnableWeaponTrail(true);
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

        if (ShouldDisableFlamethrower())
        {
            DisableFlamethrower();
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    private bool ShouldDisableFlamethrower()
    {
        return stateTimer < 0 && (enemy.bossWeaponType == BossWeaponType.Flamethrower);
    }

    public void DisableFlamethrower()
    {
        if (enemy.flamethrowActive == false)
        {
            return;
        }

        enemy.ActivateFlameThrower(false);
    }


    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (enemy.bossWeaponType == BossWeaponType.Flamethrower)
        {
            enemy.ActivateFlameThrower(true);
            enemy.bossVisuals.DischargeBatteries();
            enemy.bossVisuals.EnableWeaponTrail(false);
        }
        else if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {

            enemy.ActivateHummer();
        }
    }
}
