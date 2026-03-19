using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_EnemyStateMachine
{
    public VT_EnemyState currentState {  get; private set; }

    public void Initialize(VT_EnemyState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(VT_EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
