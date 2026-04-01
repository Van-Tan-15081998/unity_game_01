using UnityEngine;

public class VT_Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private float impactForce;

    private Rigidbody rb;
    private BoxCollider cd;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private LayerMask allyLayerMask;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cd = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(LayerMask allyLayerMark, float flyDistance = 100, float impactForce = 100)
    {
        this.allyLayerMask = allyLayerMark;
        this.impactForce = impactForce;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear();
        trailRenderer.time = .25f; ///// Vid-74
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f; /// + .5f để đúng bằng khoảng cách của Laser (tip)
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        /// Nếu viên đạn bay vượt khoảng cách cho phép thì trả về Pool (thu hồi)
        if (trailRenderer.time < 0)
        {
            ReturnBulletToPool();
        }
    }

    protected void DisableBulletIfNeeded()
    {
        /// Nếu viên đạn bay vượt khoảng cách cho phép thì vô hiệu hóa (disable) viên đạn
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && (bulletDisabled == false))
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected void FadeTrailIfNeeded()
    {
        /// Trước khi viên đạn chạm đến điểm bay giới hạn => giảm thời gian (tồn tại) của trailRenderer
        /// => Tạo hiệu ứng biến mất mượt và không gây tức thời của trailRenderer
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime; ///// Vid-74
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (FriendlyFire() == false)
        {
            /// So sánh Layer để xác định đồng minh
           if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(10);
                return; 
            }
        }

        /// Tạo hiệu ứng va chạm tại điểm va chạm
        CreateImpactFX();

        /// Trả viên đạn về Object Pool sau khi va chạm để tái sử dụng
        ReturnBulletToPool();

        /// [Damage_System]
        VT_IDamagable damagable = collision.gameObject.GetComponent<VT_IDamagable>();
        damagable?.TakeDamage();


        ///
        VT_EnemyShield shield = collision.gameObject.GetComponentInParent<VT_EnemyShield>();

        if (shield != null)
        {
            shield.ReduceDurability();
            return;
        }

        ///
        ApplyBulletImpactToEnemy(collision);
    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        VT_Enemy enemy = collision.gameObject.GetComponentInParent<VT_Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidbody);
        }
    }

    protected void ReturnBulletToPool(float delay = 0)
    {
        VT_ObjectPool.instance.ReturnObject(gameObject, delay);
    }

    protected void CreateImpactFX()
    {
        GameObject newImpactFx = VT_ObjectPool.instance.GetObject(bulletImpactFX, transform);
        VT_ObjectPool.instance.ReturnObject(newImpactFx, 1);
    }

    private bool FriendlyFire()
    {
        return VT_GameManager.instance.friendlyFire;
    }
}
