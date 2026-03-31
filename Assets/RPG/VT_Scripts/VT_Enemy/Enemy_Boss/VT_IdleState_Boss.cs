public class VT_IdleState_Boss : VT_EnemyState
{
    private VT_Enemy_Boss enemy;

    public VT_IdleState_Boss(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        /// Boss đứng tại chỗ vẫn có thể tấn công Player nếu đủ điều kiện
        if (enemy.inBattleMode && enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        /// Chuyển sang [MoveState] nếu kết thúc [IdleTime]
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
