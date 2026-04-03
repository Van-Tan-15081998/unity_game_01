using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum BossWeaponType { Flamethrower, Hummer };

public class VT_Enemy_Boss : VT_Enemy
{
    [Header("Boss Details")]
    public BossWeaponType bossWeaponType;
    public float actionCooldown = 10;
    public float attackRange;

    [Header("Ability")]
    public float minAbilityDistance;
    public float abilityCooldown;
    private float lastTimeUsedAbility;

    [Header("Flamethrower")]
    public int flameDamage;
    public float flameDamageCooldown = 0.5f;
    public ParticleSystem flamethrower;
    public float flameThrowDuration;
    public bool flamethrowActive { get; private set; }

    [Header("Hummer")]
    public int hummerActiveDamage;
    public GameObject activationPrefab;
    [SerializeField] private float hummerCheckRadius = 1.25f;


    [Header("Jump Attack")]
    public int jumpAttackDamage;
    public float jumpAttackCooldown = 10;
    private float lastTimeJumped;
    public float travelTimeToTarget = 1;
    public float minJumpDistanceRequired;

    [Space]
    public float impactRadius = 2.5f;
    public float impactPower = 15;
    public Transform impactPoint;
    [SerializeField] private float upforceMultiplier = 10;

    [Space]
    [SerializeField] private LayerMask whatToIgnore;

    [Header("Attack")]
    [SerializeField] private int meleeAttackDamage;
    [SerializeField] private Transform[] damagePoints;
    [SerializeField] private float attackCheckRadius;
    [SerializeField] private GameObject meleeAttackFX;


    public VT_IdleState_Boss idleState { get; private set; }

    public VT_MoveState_Boss moveState { get; private set; }
    public VT_AttackState_Boss attackState { get; private set; }
    public VT_JumpAttackState_Boss jumpAttackState { get; private set; }
    public VT_AbilityState_Boss abilityState { get; private set; }
    public VT_DeadState_Boss deadState { get; private set; }

    public VT_Enemy_BossVisuals bossVisuals { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        bossVisuals = GetComponent<VT_Enemy_BossVisuals>();

        idleState = new VT_IdleState_Boss(this, stateMachine, "VT_Idle");
        moveState = new VT_MoveState_Boss(this, stateMachine, "VT_Move");
        attackState = new VT_AttackState_Boss(this, stateMachine, "VT_Attack");
        jumpAttackState = new VT_JumpAttackState_Boss(this, stateMachine, "VT_JumpAttack");
        abilityState = new VT_AbilityState_Boss(this, stateMachine, "VT_Ability");
        deadState = new VT_DeadState_Boss(this, stateMachine, "VT_Idle");
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

        MeleeAttackCheck(damagePoints, attackCheckRadius, meleeAttackFX, meleeAttackDamage);
    }

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }


    public override void EnterBattleMode()
    {
        if (inBattleMode)
        {
            return;
        }

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

    public void ActivateHummer()
    {
        GameObject newActivation = VT_ObjectPool.instance.GetObject(activationPrefab, impactPoint);

        VT_ObjectPool.instance.ReturnObject(newActivation, 1);

        MassDamage(damagePoints[0].position, hummerCheckRadius, hummerActiveDamage);
    }

    public bool CanDoAbility()
    {
        bool playerWithinDistance = Vector3.Distance(transform.position, player.position) < minAbilityDistance;

        /// Nếu vị trí Player nằm ngoài tầm của Ability
        if (playerWithinDistance == false)
        {
            return false;
        }

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

    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint;
        if (impactPoint == null)
        {
            impactPoint = transform;
        }

        MassDamage(impactPoint.position, impactRadius, jumpAttackDamage);
    }

    private void MassDamage(Vector3 impactPoint, float impactRadius, int damage)
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius, ~whatIsAlly);

        foreach (Collider collider in colliders)
        {
            VT_IDamagable damagable = collider.GetComponent<VT_IDamagable>();

            if (damagable != null)
            {
                GameObject rootEntity = collider.transform.root.gameObject;

                if (uniqueEntities.Add(rootEntity) == false)
                {
                    continue;
                }

                damagable.TakeDamage(damage);
            }

            ///
            ApplyPhysicalForceTo(impactPoint, impactRadius, collider);
        }
    }

    private void ApplyPhysicalForceTo(Vector3 impactPoint, float impactRadius, Collider collider)
    {
        /// Lực tác động lên các vật thể xung quanh
        Rigidbody rb = collider.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddExplosionForce(
                impactPower, impactPoint, impactRadius, upforceMultiplier, ForceMode.Impulse);
        }
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
            if (hit.transform.root == player.root) /// || hit.transform.parent == player)
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0); /// Lấy vị trí của Enemy nhưng cao 1.5
            Vector3 playerPos = player.position + Vector3.up;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(myPos, playerPos);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);

        if (damagePoints.Length > 0)
        {
            foreach (var damagePoint in damagePoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(damagePoint.position, attackCheckRadius);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(damagePoints[0].position, hummerCheckRadius);
        }
    }
}
