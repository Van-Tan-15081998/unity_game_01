using UnityEngine;

public class VT_Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;


    private Vector3 direction;

    private Transform player;
    private float flySpeed = 2.5f;
    private float rotationSpeed = 1600;
    private float timer = 1;

    private int damage;

    public void AxeSetup(float flySpeed, Transform player, float timer, int damage)
    {
        this.damage = damage;
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void Update()
    {

        /// Xoay Axe
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        /// Giới hạn khoảng thời gian Axe hướng về Player
        if (timer > 0)
        {
            direction = player.position + Vector3.up - transform.position;

        }


        transform.forward = rb.velocity;
    }

    private void FixedUpdate()
    {
        /// Cài đặt tốc độ bay của Axe trong FixedUpdate sẽ giúp di chuyển mượt mà hơn
        /// Hiệu ứng Trail cũng sẽ mượt hơn
        rb.velocity = direction.normalized * flySpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        VT_IDamagable damagable = collision.gameObject.GetComponent<VT_IDamagable>();
        damagable?.TakeDamage(damage);

        /// Tạo hiệu ứng va chạm
        GameObject newFx = VT_ObjectPool.instance.GetObject(impactFx, transform);

        VT_ObjectPool.instance.ReturnObject(gameObject);
        VT_ObjectPool.instance.ReturnObject(newFx, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        /// Không sử dụng OnTriggerEnter nữa
        /// Vì cần Axe bị phá hủy ngay lập tức ngay khi vừa Va Chạm
        /// Nên chỉ cần xử lý trong hàm OnCollisionEnter()
        /// 

        return;
        
        //VT_IDamagable damagable = other.GetComponent<VT_IDamagable>();

        //if (damagable != null)
        //{
        //    /// Tạo hiệu ứng va chạm
        //    GameObject newFx = VT_ObjectPool.instance.GetObject(impactFx, transform);

        //    VT_ObjectPool.instance.ReturnObject(gameObject);
        //    VT_ObjectPool.instance.ReturnObject(newFx, 1f);
        //}

    }
}
