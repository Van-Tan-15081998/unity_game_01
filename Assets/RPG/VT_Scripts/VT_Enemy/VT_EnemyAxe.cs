using UnityEngine;

public class VT_EnemyAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;


    private Vector3 direction;

    private Transform player;
    private float flySpeed = 2.5f;
    private float rotationSpeed = 1600;
    private float timer = 1;

    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
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

        rb.velocity = direction.normalized * flySpeed;

        transform.forward = rb.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        VT_Bullet bullet = other.GetComponent<VT_Bullet>(); 
        VT_Player player = other.GetComponent<VT_Player>();

        if (bullet != null || player != null) 
        {
            /// Tạo hiệu ứng va chạm
            GameObject newFx = VT_ObjectPool.instance.GetObject(impactFx, transform);

            VT_ObjectPool.instance.ReturnObject(gameObject);
            VT_ObjectPool.instance.ReturnObject(newFx, 1f);
        }
    }
}
