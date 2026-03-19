using UnityEngine;

public class VT_Enemy_Melee : VT_Enemy
{
    public VT_IdleState_Melee idleState { get; private set; }
    public VT_MoveState_Melee moveState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new VT_IdleState_Melee(this, stateMachine, "Idle");
        moveState = new VT_MoveState_Melee(this, stateMachine, "Move");
    }


    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        
    }
}
