using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_Range : VT_Enemy
{
    public Transform weaponHolder;
    public VT_Enemy_RangeWeaponType weaponType;

    public float fireRate = 1; /// Bullets per second
    public GameObject bulletPrefab;
    public Transform gunPoint;
    public float bulletSpeed = 20;
    public int bulletToShot = 5; /// Bullets to shoot before weapon goes on cooldown
    public float weaponCooldown = 1.5f; /// Weapon cooldown after all bullets shot


    public VT_IdleState_Range idleState { get; private set; }
    public VT_MoveState_Range moveState { get; private set; }
    public VT_BattleState_Range battleState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new VT_IdleState_Range(this, stateMachine, "VT_Idle");
        moveState = new VT_MoveState_Range(this, stateMachine, "VT_Move");
        battleState = new VT_BattleState_Range(this, stateMachine, "VT_Battle");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(startState: idleState);

        visuals.SetupLook();
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

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

        rbNewBullet.mass = 20 / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;

    
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
        {
            return;
        }

        base.EnterBattleMode();
        stateMachine.ChangeState(battleState);

    }
}
