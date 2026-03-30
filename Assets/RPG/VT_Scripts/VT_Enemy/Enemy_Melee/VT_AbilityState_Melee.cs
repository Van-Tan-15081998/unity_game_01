using UnityEngine;

public class VT_AbilityState_Melee : VT_EnemyState
{
    private VT_Enemy_Melee enemy;
    private Vector3 movementDirection;

    private const float MAX_MOVEMENT_DISTANCE = 20f;

    private float moveSpeed;
    //private float lastTimeAxeThrow;

    public VT_AbilityState_Melee(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableWeaponModel(true);

        moveSpeed = enemy.walkSpeed;
        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.walkSpeed = moveSpeed;
        enemy.anim.SetFloat("VT_RecoveryIndex", 0);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.FaceTarget(enemy.player.position);
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        }

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.walkSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.recoveryState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        //if (Time.time < enemy.axeThrowCooldown + lastTimeAxeThrow)
        //{
        //    return;
        //}
        //lastTimeAxeThrow = Time.time;

        ///
        GameObject newAxe = VT_ObjectPool.instance.GetObject(enemy.axePrefab);

        newAxe.transform.position = enemy.axeStartPoint.position;
        newAxe.GetComponent<VT_EnemyAxe>().AxeSetup(enemy.axeFlySpeed, enemy.player, enemy.axeAimTimer);
    }
}
