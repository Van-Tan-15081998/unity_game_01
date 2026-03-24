using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_DeadState : VT_EnemyState
{
    private VT_Enemy_Melee enemy;
    private VT_EnemyRagdoll ragdoll;

    private bool interactionDisabled;

    public VT_DeadState(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as VT_Enemy_Melee;
        ragdoll = enemy.GetComponent<VT_EnemyRagdoll>();    
    }

    public override void Enter()
    {
        base.Enter();

        interactionDisabled = false;

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        /// Vào trạng thái Dead => isKinematic = false => Chịu tác động của Vật lý
        ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        // DisableInteractionIfShould(); /// Mở Comment nếu muốn thay đổi mục đích
    }

    private void DisableInteractionIfShould()
    {
        /// Tắt khả năng tương tác vật lý 
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            ragdoll.RagdollActive(false);
            ragdoll.CollidersActive(false);
        }
    }
}
