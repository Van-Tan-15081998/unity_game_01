using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_BattleState_Range : VT_EnemyState
{
    private VT_Enemy_Range enemy;

    private float lastTimeShot = -10;
    private int bulletsShot = 0;

    public VT_BattleState_Range(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        
    }

    public override void Exit()
    {
        base.Exit();


    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (WeaponOutOfBullets())
        {
            if (WeaponOnCooldown())
            {
                AttempToResetWeapon();
            }

            return;
        }

        if (CanShoot())
        {
            Shoot();
        }
    }

    private void AttempToResetWeapon()
    {
        bulletsShot = 0;
    }

    private bool WeaponOnCooldown()
    {
        return Time.time > lastTimeShot + enemy.weaponCooldown;
    }

    private bool WeaponOutOfBullets()
    {
        return bulletsShot >= enemy.bulletToShot;
    }

    private bool CanShoot()
    {
        return Time.time > lastTimeShot + 1 / enemy.fireRate;
    }

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }
}
