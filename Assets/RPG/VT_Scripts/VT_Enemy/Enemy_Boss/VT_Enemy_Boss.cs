using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_Boss : VT_Enemy
{
    [Header("Boss Details")]
    public float actionCooldown = 10;
    public float attackRange;

    [Header("Ability")]
    public ParticleSystem flamethrower;
    public float abilityCooldown;
    private float lastTimeUsedAbility;
    public float flameThrowDuration;

    public bool flamethrowActive {  get; private set; } 


    [Header("Jump Attack")]
    public float jumpAttackCooldown = 10;
    private float lastTimeJumped;
    public float travelTimeToTarget = 1;
    public float minJumpDistanceRequired;
    [Space]
    [SerializeField] private LayerMask whatToIgnore;


    public VT_IdleState_Boss idleState { get; private set; }

    public VT_MoveState_Boss moveState { get; private set; }
    public VT_AttackState_Boss attackState { get; private set; }
    public VT_JumpAttackState_Boss jumpAttackState { get; private set; }
    public VT_AbilityState_Boss abilityState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new VT_IdleState_Boss(this, stateMachine, "VT_Idle");
        moveState = new VT_MoveState_Boss(this, stateMachine, "VT_Move");
        attackState = new VT_AttackState_Boss(this, stateMachine, "VT_Attack");
        jumpAttackState = new VT_JumpAttackState_Boss(this, stateMachine, "VT_JumpAttack");
        abilityState = new VT_AbilityState_Boss(this, stateMachine, "VT_Ability");
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

        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    public override void EnterBattleMode()
    {
        base.EnterBattleMode();

        stateMachine.ChangeState(moveState);
    }



    public void ActivateFlameThrower(bool activate)
    {
        flamethrowActive = activate;

        if (!activate)
        {
            flamethrower.Stop();
            anim.SetTrigger("VT_StopFlameThrower");
            return;
        }

        var mainModule = flamethrower.main;
        var extraModule = flamethrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        /// .GetChild(0) => Tức là [FireEmbers] (FlameStream > FireEmbers)

        mainModule.duration = flameThrowDuration;
        extraModule.duration = flameThrowDuration;

        flamethrower.Clear();
        flamethrower.Play();
    }

    public bool CanDoAbility()
    {
        if (Time.time > lastTimeUsedAbility + abilityCooldown)
        {
            
            return true;
        }

        return false;   
    }

    public void SetAbilityOnCooldown()
    {
        lastTimeUsedAbility = Time.time;
    }

    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < minJumpDistanceRequired)
        {
            return false;
        }

        /// Kết hợp điều kiện Enemy nhìn thấy Player
        if (Time.time > lastTimeJumped + jumpAttackCooldown && IsPlayerInClearSight())
        {
            
            return true;
        }

        return false;   
    }

    public void SetJumpAttackOnCooldown()
    {
        lastTimeJumped = Time.time;
    }

    public bool IsPlayerInClearSight()
    {
        Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0); /// Lấy vị trí của Enemy nhưng cao 1.5
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPos - myPos).normalized; 

        if (Physics.Raycast(myPos,directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            if (hit.transform == player || hit.transform.parent == player)
            {
                return true;
            }
        }

        return false;
    }

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;

        if (player != null)
        {
            Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0); /// Lấy vị trí của Enemy nhưng cao 1.5
            Vector3 playerPos = player.position + Vector3.up;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(myPos, playerPos);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);
    }
}
