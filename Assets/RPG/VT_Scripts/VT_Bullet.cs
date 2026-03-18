using UnityEngine;

public class VT_Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private Rigidbody rb;
    private BoxCollider cd;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cd = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(float flyDistance)
    {
        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = .25f; ///// Vid-74
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f; /// + .5f để đúng bằng khoảng cách của Laser (tip)
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        /// Nếu viên đạn bay vượt khoảng cách cho phép thì trả về Pool (thu hồi)
        if (trailRenderer.time < 0)
        {
            ReturnBulletToPool();
        }
    }

    private void DisableBulletIfNeeded()
    {
        /// Nếu viên đạn bay vượt khoảng cách cho phép thì vô hiệu hóa (disable) viên đạn
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && (bulletDisabled == false))
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        /// Trước khi viên đạn chạm đến điểm bay giới hạn => giảm thời gian (tồn tại) của trailRenderer
        /// => Tạo hiệu ứng biến mất mượt và không gây tức thời của trailRenderer
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime; ///// Vid-74
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        /// Tạo hiệu ứng va chạm tại điểm va chạm
        CreateImpactFX(collision);

        /// Trả viên đạn về Object Pool sau khi va chạm để tái sử dụng
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool()
    {
        VT_ObjectPool.instance.ReturnObject(gameObject);
    }

    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            // Lấy thông tin va chạm từ contact point đầu tiên
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

            Destroy(newImpactFX, 1f);
        }
    }
}
