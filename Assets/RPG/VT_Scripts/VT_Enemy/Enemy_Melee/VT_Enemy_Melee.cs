using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VT_AttackData_EnemyMelee
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 2)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee
{
    Close, Charge
}

public enum EnemyMelee_Type { Regular, Shield, DodgeRoll, AxeThrow }

public class VT_Enemy_Melee : VT_Enemy
{
    


    public VT_IdleState_Melee idleState { get; private set; }
    public VT_MoveState_Melee moveState { get; private set; }
    public VT_RecoveryState_Melee recoveryState { get; private set; }
    public VT_ChaseState_Melee chaseState { get; private set; }
    public VT_AttackState_Melee attackState { get; private set; }
    public VT_DeadState deadState { get; private set; }
    public VT_AbilityState_Melee abilityState { get; private set; }

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType;
    public VT_Enemy_MeleeWeaponType weaponType;

    public Transform shieldTransform;
    public float dodgeRollCooldown;
    private float lastTimeDodgeRoll = -10; /// Giá trị mặc định => đảm bảo luôn có thể thực hiện ngay lần đầu tiên Check điều kiện

    [Header("Axe Throw Ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    public Transform axeStartPoint; /// Vị trí xuất phát của Axe
    private float lastTimeAxeThrown;

    [Header("Attack Data")]
    public VT_AttackData_EnemyMelee attackData;
    public List<VT_AttackData_EnemyMelee> attackList;

    //[SerializeField] private Transform hiddenWeapon;
    //[SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        

        idleState = new VT_IdleState_Melee(this, stateMachine, "VT_Idle");
        moveState = new VT_MoveState_Melee(this, stateMachine, "VT_Move");
        recoveryState = new VT_RecoveryState_Melee(this, stateMachine, "VT_Recovery");
        chaseState = new VT_ChaseState_Melee(this, stateMachine, "VT_Chase");
        attackState = new VT_AttackState_Melee(this, stateMachine, "VT_Attack");
        deadState = new VT_DeadState(this, stateMachine, "VT_Idle"); /// Idle anim just a place holder, we use ragdoll
        abilityState = new VT_AbilityState_Melee(this, stateMachine, "VT_AxeThrow");
    }


    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        InitializePerk();

        visuals.SetupLook();

        ///
        UpdateAttackData();

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();


    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
        {
            return; 
        }

        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);

    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        /// Cài đặt tốc độ di chuyển trong lúc thực hiện động tác => chậm hơn bình thường
        walkSpeed = walkSpeed * .6f;

        /// Tắt hiện thị vũ khí đang cầm trên tay
        EnableWeaponModel(false);
    }

    public void UpdateAttackData()
    {
        VT_Enemy_WeaponModel currentWeapon = visuals.currentWeaponModel.GetComponent<VT_Enemy_WeaponModel>();

        if (currentWeapon.weaponData != null)
        {
            attackList = new List<VT_AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    private void InitializePerk()
    {
        /// AxeThrow => Chỉ trang bị Axe 
        /// Hiển thị loại vũ khí dựa trên [EnemyMelee_Type]
        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            weaponType = VT_Enemy_MeleeWeaponType.Throw;
        }

        /// Shield => Một tay trang bị Shield & Một tay còn lại trang bị một vũ khí không phải là Axe
        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("VT_ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);

            /// Hiển thị loại vũ khí dựa trên [EnemyMelee_Type]
            weaponType = VT_Enemy_MeleeWeaponType.OneHand;
        }

        /// DodgeRoll (Né tránh) => Không trang bị vũ khí - Tay không 
        if (meleeType == EnemyMelee_Type.DodgeRoll)
        {
            weaponType = VT_Enemy_MeleeWeaponType.Unarmed;
        }
    }

    public override void GetHit()
    {
        base.GetHit();

        if (healthPoints <= 0)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public void EnableWeaponModel(bool active)
    {
        visuals.currentWeaponModel.gameObject.SetActive(active);
    }

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackData.attackRange;
    }

    public void ActivateDodgeRoll()
    {
        if (meleeType != EnemyMelee_Type.DodgeRoll)
        {
            return;
        }

        if (stateMachine.currentState != chaseState)
        {
            return;
        }

        /// Nếu khoảng cách giữa enemy vs player quá gần => không thực hiện động tác né tránh
        if (Vector3.Distance(transform.position, player.position) < 2f)
        {
            return;
        }

        /// Điều kiện dưới đây không chỉ phụ thuộc vào [dodgeRollCooldown] mà còn duration (length) của Animation - DodgeRoll
        /// Thực thi sớm hơn => Bug
        /// => Thực thi cả sau khi Animation - DodgeRoll hoàn thành
        /// Hard code: Vào xem length của Animation - DodgeRoll
        /// Hoặc dùng hàm sau:
        float dodgeAnimationDuration = GetAnimationClipDuration("DodgeRoll");

        if (Time.time > dodgeRollCooldown + dodgeAnimationDuration + lastTimeDodgeRoll)
        {
            lastTimeDodgeRoll = Time.time;
            anim.SetTrigger("VT_DodgeRoll");
        }

    }

    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow)
        {
            return false;
        }

        if (Time.time > lastTimeAxeThrown + axeThrowCooldown)
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }

        return false;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}
