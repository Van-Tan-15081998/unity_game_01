using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public enum CoverPerk { Unavalible, CanTakeCover, CanTakeAndChangeCover}

public enum UnstoppablePerk { Unavalible, Unstoppable }

public enum GrenadePerk { Unavalible, CanThrowGrenade };

public class VT_Enemy_Range : VT_Enemy
{
    [Header("Enemy Perks")]
    public CoverPerk coverPerk;
    public UnstoppablePerk unstoppablePerk;
    public GrenadePerk grenadePerk;

    [Header("Grenade Perks")]
    public GameObject grenadePrefab;
    public float impactPower = 5f;
    public float explosionTimer = .75f;
    public float timeToTarget = 1.2f;
    public float grenadeCooldown;
    private float lastTimeGrenadeThrown = -10;
    [SerializeField] private Transform grenadeStartPoint;

    [Header("Advance Perks")]
    public float advanceSpeed;
    public float advanceStoppingDistance;
    public float advanceDuration = 2.5f;

    [Header("Cover System")]
    public float minCoverTime = 3.0f; /// Thời gian tối thiểu Enemy ẩn nấp sau vật chắn 
    public float safeDistance;
    public VT_CoverPoint currentCover { get; private set; }
    public VT_CoverPoint lastCover { get; private set; }

    [Header("Weapon Details")]
    public float attackDelay = 1.0f;
    public VT_Enemy_RangeWeaponType weaponType;
    public VT_Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [Header("Aim Details")]
    public float slowAim = 4;
    public float fastAim = 20;
    public Transform aim;
    public Transform playerBody;
    public LayerMask whatToIgnore;

    [SerializeField] List<VT_Enemy_RangeWeaponData> avalibleWeaponData;

    public VT_IdleState_Range idleState { get; private set; }

    public VT_MoveState_Range moveState { get; private set; }

    public VT_BattleState_Range battleState { get; private set; }

    public VT_RunToCoverState_Range runToCoverState { get; private set; }
    
    public VT_AdvancePlayerState_Range advancePlayerState { get; private set; }
    public VT_ThrowGrenadeState_Range throwGrenadeState { get; private set; }
    public VT_DeadState_Range deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new VT_IdleState_Range(this, stateMachine, "VT_Idle");
        moveState = new VT_MoveState_Range(this, stateMachine, "VT_Move");
        battleState = new VT_BattleState_Range(this, stateMachine, "VT_Battle");
        runToCoverState = new VT_RunToCoverState_Range(this, stateMachine, "VT_RunToCover");
        advancePlayerState = new VT_AdvancePlayerState_Range(this, stateMachine, "VT_AdvancePlayer");
        throwGrenadeState = new VT_ThrowGrenadeState_Range(this, stateMachine, "VT_ThrowGrenade");

        deadState = new VT_DeadState_Range(this, stateMachine, "VT_Idle"); /// VT_Idle is a place holder, we using ragdoll

    }

    protected override void Start()
    {
        base.Start();

        playerBody = player.GetComponent<VT_Player>().playerBody;
        aim.parent = null;

        InitializePerk();

        stateMachine.Initialize(startState: idleState);

        visuals.SetupLook();

        SetupWeapon();

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    //public override void GetHit()
    //{
    //    base.GetHit();

    //    if (healthPoints <= 0 && stateMachine.currentState != deadState)
    //    {
    //        stateMachine.ChangeState(deadState);    
    //    }
    //}

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public bool CanThrowGrenade()
    {
        if (grenadePerk == GrenadePerk.Unavalible)
        {
            return false;
        }

        if (Vector3.Distance(player.transform.position, transform.position) < safeDistance)
        {
            return false;
        }

        if (Time.time > grenadeCooldown + lastTimeGrenadeThrown)
        {
            return true;
        }

        return false;
    }

    public void ThrowGrenade()
    {
        lastTimeGrenadeThrown = Time.time;
        visuals.EnableGrenadeModel(false);

        GameObject newGrenade = VT_ObjectPool.instance.GetObject(grenadePrefab, grenadeStartPoint);

        VT_Enemy_Grenade newGrenadeScript = newGrenade.GetComponent<VT_Enemy_Grenade>();

        if (stateMachine.currentState == deadState)
        {
            newGrenadeScript.SetupGrenade(transform.position, 1, explosionTimer, impactPower);
            return;
        }

        //Debug.LogWarning("PlayerPosition: " + player.transform.position.y);
        newGrenadeScript.SetupGrenade(player.transform.position, timeToTarget, explosionTimer, impactPower);
    }

    protected override void InitializePerk()
    {
        if (IsUnstoppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("VT_AdvanceAnimIndex", 1); /// 1 is a slow walk animation
        }

    }

    #region Cover System 

    public bool CanGetCover()
    {
        if (coverPerk == CoverPerk.Unavalible)
        {
            return false;
        }

        currentCover = AttemptToFindCover()?.GetComponent<VT_CoverPoint>();

        if (lastCover != currentCover && currentCover != null)
        {
            return true;
        }

        //Debug.LogWarning("Không tìm thấy điểm ẩn nấp!");

        return false;   
    }

    private Transform AttemptToFindCover()
    {
        List<VT_CoverPoint> collectedCoverPoints = new List<VT_CoverPoint>();

        foreach (VT_Cover cover in CollectNearByCovers())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        VT_CoverPoint closestCoverPoint = null;
        float shortestDistance = float.MaxValue;

        foreach (VT_CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if (currentDistance < shortestDistance)
            {
                closestCoverPoint = coverPoint;
                shortestDistance = currentDistance;
            }
        }

        if (closestCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;

            currentCover = closestCoverPoint;
            currentCover?.SetOccupied(true);

            return currentCover.transform;
        }

        return null;
    }

    private List<VT_Cover> CollectNearByCovers()
    {
        float coverRadiusCheck = 30;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRadiusCheck);
        List<VT_Cover> collectedCovers = new List<VT_Cover>();

        foreach (Collider collider in hitColliders)
        {
            VT_Cover cover = collider.GetComponent<VT_Cover>();

            if (cover != null && collectedCovers.Contains(cover) == false)
            {
                collectedCovers.Add(cover);
            }
        }

        return collectedCovers;
    }

    #endregion

    public void FireSingleBullet()
    {

        anim.SetTrigger("VT_Shoot");

        //Vector3 bulletsDirection = ((player.position + Vector3.up) - gunPoint.position).normalized; /// Or
        Vector3 bulletsDirection = (aim.position - gunPoint.position).normalized;

        GameObject newBullet = VT_ObjectPool.instance.GetObject(bulletPrefab, gunPoint);
        
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<VT_Bullet>().BulletSetup(whatIsAlly);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.velocity = bulletDirectionWithSpread * weaponData.bulletSpeed;


    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
        {
            return;
        }

        base.EnterBattleMode();

        if (CanGetCover())
        {
            stateMachine.ChangeState(runToCoverState);
        }
        else
        {
            stateMachine.ChangeState(battleState);
        }


    }

    private void SetupWeapon()
    {
        List<VT_Enemy_RangeWeaponData> filteredData = new List<VT_Enemy_RangeWeaponData>();

        foreach (var weaponData in avalibleWeaponData)
        {
            if (weaponData.weaponType == weaponType)
            {
                filteredData.Add(weaponData);
            }
        }

        if (filteredData.Count > 0)
        {
            int random = Random.Range(0, filteredData.Count);
            weaponData = filteredData[random];
        }
        else
        {
            //Debug.LogWarning("Không tìm thấy thông tin về vũ khí!");
        }

        gunPoint = visuals.currentWeaponModel.GetComponent<VT_Enemy_RangeWeaponModel>().gunPoint;
    }

    //protected override void OnDrawGizmos()
    //{
    //    base.OnDrawGizmos();
    //    Gizmos.DrawLine(transform.position, player.transform.position); 
    //}

    #region Enemy's Aim
    public void UpdateAimPosition()
    {
        float aimSpeed = IsAimOnPlayer() ? fastAim : slowAim;
        aim.position = Vector3.MoveTowards(aim.position, playerBody.position, aimSpeed * Time.deltaTime);
    }

    public bool IsAimOnPlayer()
    {
        /// Aim component sẽ gắn với vị trí của Player khi bản thân enemy có thể nhìn thấy 
        /// và nhắm mục tiêu vào Player;
        /// Nhắm chỉ còn hiệu lực khi khoảng cách giữa [Aim component] và [Player body] < 2m

        float distanceAimToPlayer = Vector3.Distance(aim.position, player.position);

        //Debug.LogWarning("Aim on Player: " + distanceAimToPlayer + "___" + Time.time.ToString());

        /// Nếu Player gần [Aim component] thì Aim có hiệu lực, dù cho cả khi Player đứng sau vật chắn
        return distanceAimToPlayer < 2;
    }

    public bool IsSeeingPlayer()
    {
        Vector3 myPosition = transform.position + Vector3.up;

        Vector3 directionToPlayer = playerBody.position - myPosition;

        if (Physics.Raycast(myPosition, directionToPlayer, out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
        {
            
            if (hit.transform == player)
            {
                /// player chính là [VT_Player Component].transform
                /// [VT_Player Component] có [Capsule Collider] <=> Nếu va chạm => hit = [VT_Player Component]

                //Debug.LogWarning("Enemy nhìn thấy Player! "
                //    + hit.transform.name
                //    + ""
                //    + Time.time.ToString());

                UpdateAimPosition();

                return true;
            }
            else
            {
                //Debug.LogWarning("Enemy không nhìn thấy Player! " +
                //    "\nVật thể trung gian giữa Enemy và Player: "
                //    + hit.transform.name
                //    + ""
                //    + Time.time.ToString());
            }
        }

        return false;
    }
    #endregion

    public bool IsUnstoppable()
    {
        return unstoppablePerk == UnstoppablePerk.Unstoppable;
    }
}
