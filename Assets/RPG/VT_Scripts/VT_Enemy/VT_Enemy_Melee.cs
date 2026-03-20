using UnityEngine;

public class VT_Enemy_Melee : VT_Enemy
{
    public VT_IdleState_Melee idleState { get; private set; }
    public VT_MoveState_Melee moveState { get; private set; }
    public VT_RecoveryState_Melee recoveryState { get; private set; }
    public VT_ChaseState_Melee chaseState { get; private set; }
    public VT_AttackState_Melee attackState { get; private set; }

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        idleState = new VT_IdleState_Melee(this, stateMachine, "VT_Idle");
        moveState = new VT_MoveState_Melee(this, stateMachine, "VT_Move");
        recoveryState = new VT_RecoveryState_Melee(this, stateMachine, "VT_Recovery");
        chaseState = new VT_ChaseState_Melee(this, stateMachine, "VT_Chase");
        attackState = new VT_AttackState_Melee(this, stateMachine, "VT_Attack");
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

    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pulledWeapon.gameObject.SetActive(true);
    }
}
