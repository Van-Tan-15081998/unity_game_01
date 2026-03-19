using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_EnemyState
{
    protected VT_Enemy enemyBase;
    protected VT_EnemyStateMachine stateMachine;

    protected string animBoolName;
    public float stateTimer;

    public VT_EnemyState(VT_Enemy enemyBase, VT_EnemyStateMachine stateMachine, string animBoolName )
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {

    }

    public virtual void Update() 
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {

    }
}
