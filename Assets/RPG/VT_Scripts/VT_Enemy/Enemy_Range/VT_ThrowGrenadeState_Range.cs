using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_ThrowGrenadeState_Range : VT_EnemyState
{
    private VT_Enemy_Range enemy;
    public bool finishedThrowingGrenade {  get; private set; }  

    public VT_ThrowGrenadeState_Range(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        finishedThrowingGrenade = false;

        enemy.visuals.EnableWeaponModel(false);
        enemy.visuals.EnableIK(false, false);

        enemy.visuals.EnableSecondaryWeaponModel(true);

        enemy.visuals.EnableGrenadeModel(true);
    }

    public override void Exit()
    {
        base.Exit();

        //enemy.visuals.EnableWeaponModel(true);
        //enemy.visuals.EnableSecondaryWeaponModel(false);
    }

    public override void Update()
    {
        base.Update();

        Vector3 playerPos = enemy.player.position + Vector3.up;

        enemy.FaceTarget(playerPos);
        enemy.aim.position = playerPos;

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        finishedThrowingGrenade = true;

        enemy.ThrowGrenade();

        //enemy.visuals.EnableIK(true, true, 1.5f); /// Dùng ở đây hoặc sử dụng Animation Event
        /// => Có thể áp dụng từ frame nào 
    }
}
