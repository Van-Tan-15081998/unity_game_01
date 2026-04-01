using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardsMultiplier = 1f;
    private float impactPower;
    private Rigidbody rb;
    private float timer;

    private LayerMask allyLayerMask;

    private string VT_grenadeId;
    private bool VT_explored;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && VT_explored == false)
        {
            Explode();
        }
        else
        {

        }

    }

    private void Explode()
    {
        //Debug.LogWarning("Explode()" + "ID: " + VT_grenadeId + "_____" + timer.ToString());
        VT_explored = true;

        PlayExplosionFX();

        /// [!] Đối với các object có nhiều collider như Player (chân, tay, đầu,...)
        /// => Khi chịu tác động từ vụ nổ chỉ lấy một collider để nhận sát thương
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider collider in colliders)
        {
            ///
            if (IsTargetValid(collider) == false)
            {
                continue;
            }

            ///
            GameObject rootEntity = collider.transform.root.gameObject;
            if (uniqueEntities.Add(rootEntity) == false)
            { continue; }

            ///
            ApplyDamageTo(collider);

            ///
            ApplyPhysicalForceTo(collider);
        }
    }

    private void ApplyPhysicalForceTo(Collider collider)
    {
        Rigidbody rb = collider.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddExplosionForce(
                impactPower, transform.position, impactRadius, upwardsMultiplier, ForceMode.Impulse);
        }
    }

    private static void ApplyDamageTo(Collider collider)
    {
        VT_IDamagable damagable = collider.GetComponent<VT_IDamagable>();
        damagable?.TakeDamage();
    }

    private void PlayExplosionFX()
    {
        GameObject newFX = VT_ObjectPool.instance.GetObject(explosionFX, transform);

        VT_ObjectPool.instance.ReturnObject(newFX, 1);
        VT_ObjectPool.instance.ReturnObject(gameObject);
    }

    public void SetupGrenade(LayerMask allyLayerMask, Vector3 target, float timeToTarget, float countdown, float impactPower)
    {
        this.allyLayerMask = allyLayerMask;

        rb.velocity = CalculateLaunchVelocity(target, timeToTarget);
        timer = countdown + timeToTarget;
        this.impactPower = impactPower;

        //Debug.LogWarning("SetupGrenade()" + timer.ToString());
        VT_grenadeId = timer.ToString();
        VT_explored = false;
    }

    private bool IsTargetValid(Collider collider)
    {
        // If friendly fire is enable, all colliders are valid targets
        if (VT_GameManager.instance.friendlyFire)
        {
            return true;
        }

        // If collider is on allyLayerMask, target is not valid
        if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
        {
            return false;
        }

        return true;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        Vector3 velocityXZ = directionXZ / timeToTarget;

        float velocityY =
            (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;

        return launchVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
