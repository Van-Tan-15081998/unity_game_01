using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_DeadState_Range : VT_EnemyState
{
    private VT_Enemy_Range enemy;

    private bool interactionDisabled;

    public VT_DeadState_Range(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.throwGrenadeState.finishedThrowingGrenade == false)
        {
            enemy.ThrowGrenade();
        }

        interactionDisabled = false;

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        /// Vào trạng thái Dead => isKinematic = false => Chịu tác động của Vật lý
        enemy.ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();

        DisableInteractionIfShould();
    }

    private void DisableInteractionIfShould()
    {
        /// Tắt khả năng tương tác vật lý 
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false);
            enemy.ragdoll.CollidersActive(false);
        }
    }
}
