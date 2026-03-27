using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VT_Enemy_Range : VT_Enemy
{
    [Header("Cover System")]
    public bool canUseCovers = true;
    public VT_CoverPoint currentCover { get; private set; }
    public VT_CoverPoint lastCover { get; private set; }

    [Header("Weapon Details")]
    public VT_Enemy_RangeWeaponType weaponType;
    public VT_Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [SerializeField] List<VT_Enemy_RangeWeaponData> avalibleWeaponData;


    public VT_IdleState_Range idleState { get; private set; }
    public VT_MoveState_Range moveState { get; private set; }
    public VT_BattleState_Range battleState { get; private set; }
    public VT_RunToCoverState_Range runToCoverState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new VT_IdleState_Range(this, stateMachine, "VT_Idle");
        moveState = new VT_MoveState_Range(this, stateMachine, "VT_Move");
        battleState = new VT_BattleState_Range(this, stateMachine, "VT_Battle");
        runToCoverState = new VT_RunToCoverState_Range(this, stateMachine, "VT_RunToCover");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(startState: idleState);

        visuals.SetupLook();

        SetupWeapon();

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    #region Cover System 

    public bool CanGetCover()
    {
        if (canUseCovers == false)
        {
            return false;
        }

        currentCover = AttemptToFindCover()?.GetComponent<VT_CoverPoint>();

        if (lastCover != currentCover && currentCover != null)
        {
            return true;
        }

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
        //return;

        anim.SetTrigger("VT_Shoot");

        Vector3 bulletsDirection = ((player.position + Vector3.up) - gunPoint.position).normalized;

        GameObject newBullet = VT_ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<VT_Enemy_Bullet>().BulletSetup();

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
            Debug.LogWarning("Không tìm thấy thông tin về vũ khí!");
        }

        gunPoint = visuals.currentWeaponModel.GetComponent<VT_Enemy_RangeWeaponModel>().gunPoint;
    }
}
