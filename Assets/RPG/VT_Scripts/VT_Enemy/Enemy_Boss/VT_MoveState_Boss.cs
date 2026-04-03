using UnityEngine;

public class VT_MoveState_Boss : VT_EnemyState
{
    private VT_Enemy_Boss enemy;
    private Vector3 destination;

    private float actionTimer;
    private float timeBeforeSpeedUp = 5;

    private bool speedUpActivated;

    public VT_MoveState_Boss(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        SpeedReset();
        enemy.agent.isStopped = false;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);

        actionTimer = enemy.actionCooldown;
    }

    private void SpeedReset()
    {
        speedUpActivated = false;
        enemy.anim.SetFloat("VT_MoveAnimIndex", 0);
        enemy.anim.SetFloat("VT_MoveAnimSpeedMultiplier", 1);
        enemy.agent.speed = enemy.walkSpeed;
    }

    private void SpeedUp()
    {
        enemy.agent.speed = enemy.runSpeed;
        enemy.anim.SetFloat("VT_MoveAnimIndex", 1);
        enemy.anim.SetFloat("VT_MoveAnimSpeedMultiplier", 1.5f);
        speedUpActivated = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime;

        enemy.FaceTarget(GetNextPathPoint());

        /// Bật chế độ tấn công trong trạng thái đang di chuyển (Khác Melee và Range)
        if (enemy.inBattleMode)
        {
            if(ShouldSpeedUp())
            {
                SpeedUp();
            }

            Vector3 playerPos = enemy.player.position;

            /// Đuổi theo Player
            enemy.agent.SetDestination(playerPos);

            if (actionTimer < 0)
            {
                PerformRandomAction();

            } else if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        } else
        {

            if (Vector3.Distance(enemy.transform.position, destination) < .25f)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

    }

    private void PerformRandomAction()
    {
        actionTimer = enemy.actionCooldown;

        if (Random.Range(0,2) == 0) /// Kết quả random 0 Hoặc 1
        {
            TryAbility();
        }
        else
        {
            if (enemy.CanDoJumpAttack())
            {
                stateMachine.ChangeState(enemy.jumpAttackState);
            } else if (enemy.bossWeaponType == BossWeaponType.Hummer)
            {
                /// Nếu không thể Jump => Thử lại Ability
                TryAbility();
            }
        }
    }

    private void TryAbility()
    {
        if (enemy.CanDoAbility())
        {
            stateMachine.ChangeState(enemy.abilityState);
        }
    }

    private bool ShouldSpeedUp()
    {
        if (speedUpActivated)
        {
            return false;
        }

        if (Time.time > enemy.attackState.lastTimeAttacked + timeBeforeSpeedUp)
        {
            return true;
        }

        return false;
    }
}
