using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_JumpAttackState_Boss : VT_EnemyState
{
    private VT_Enemy_Boss enemy;
    private Vector3 lastPlayerPos;

    private float jumpAttackMovementSpeed;  

    public VT_JumpAttackState_Boss(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        lastPlayerPos = enemy.player.position;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;    

        enemy.bossVisuals.PlaceLandingZone(lastPlayerPos);
        enemy.bossVisuals.EnableWeaponTrail(true);

        float distanceToPlayer = Vector3.Distance(lastPlayerPos, enemy.transform.position); 

        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget;

        enemy.FaceTarget(lastPlayerPos, 1000);

        /// Boss Hummer trong suốt động tác nhảy lên tấn công vẫn có sự di chuyển 
        if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {
            enemy.agent.isStopped = false;
            enemy.agent.speed = enemy.runSpeed;
            enemy.agent.SetDestination(lastPlayerPos); 
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemy.SetJumpAttackOnCooldown();

        enemy.bossVisuals.EnableWeaponTrail(false);
    }

    public override void Update()
    {
        base.Update();
        Vector3 myPos = enemy.transform.position;
        enemy.agent.enabled = !enemy.ManualMovementActive();

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(myPos, lastPlayerPos, jumpAttackMovementSpeed * Time.deltaTime); 
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
