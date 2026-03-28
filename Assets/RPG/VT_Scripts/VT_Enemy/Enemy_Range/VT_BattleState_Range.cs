using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_BattleState_Range : VT_EnemyState
{
    private VT_Enemy_Range enemy;

    private float lastTimeShot = -10;
    private int bulletsShot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    private float coverCheckTimer;
    //private bool firstTimeAttack = true;

    public VT_BattleState_Range(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;    

        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();

        enemy.visuals.EnableIK(true, true);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.visuals.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsSeeingPlayer())
        {
            enemy.FaceTarget(enemy.aim.position);
        }

        /// Nếu Player không trong phạm vi tấn công => Chuyển State sang Advance 
        if (enemy.IsPlayerInAggressionRange() == false && ReadyToLeaveCover())
        {
            stateMachine.ChangeState(enemy.advancePlayerState); /// VT_Comment
        }

        ChangeCoverIfShould();

        if (WeaponOutOfBullets())
        {
            if (WeaponOnCooldown())
            {
                AttempToResetWeapon();
            }

            return;
        }

        if (CanShoot() && enemy.IsAimOnPlayer())
        {
            Shoot();
        }
    }



    private void AttempToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
    }

    private bool WeaponOnCooldown()
    {
        return Time.time > lastTimeShot + weaponCooldown;
    }

    private bool WeaponOutOfBullets()
    {
        return bulletsShot >= bulletsPerAttack;
    }

    private bool CanShoot()
    {
        return Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate;
    }

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }

    #region Cover System Region

    private bool ReadyToLeaveCover()
    {
        return Time.time > enemy.minCoverTime + enemy.runToCoverState.lastTimeTookCover;
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
        {
            return;
        }

        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = .5f; /// We do cover check each .5f seconds


            if (ReadyToChangeCover())
            {
                if (enemy.CanGetCover())
                {
                    Debug.LogWarning("Thay đổi vị trí ẩn nấp!");
                    stateMachine.ChangeState(enemy.runToCoverState);
                }
            }
        }
    }

    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerClose() || IsPlayerInClearSight();

        bool advanceTimeIsOver = Time.time > enemy.advancePlayerState.lastTimeAdvanced
            + enemy.advanceTime;

        return inDanger && advanceTimeIsOver;   
    }

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position)
            < enemy.safeDistance;
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position; 

        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            return hit.collider.gameObject.GetComponentInParent<VT_Player>();
        }

        return false;
    }

    #endregion


}
