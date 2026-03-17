using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private Rigidbody rb => GetComponent<Rigidbody>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        /// Tạo hiệu ứng va chạm tại điểm va chạm
        CreateImpactFX(collision);

        /// Trả viên đạn về Object Pool sau khi va chạm để tái sử dụng
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
