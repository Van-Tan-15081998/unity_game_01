using System.Collections.Generic;
using UnityEngine;

public class VT_ObjectPool : MonoBehaviour
{
    public static VT_ObjectPool instance { get; private set; }

    [SerializeField] private GameObject objectPrefab; // Prefab của đối tượng cần pool
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> objectPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; /// Gán instance của ObjectPool khi lần đầu tiên Awake được gọi
        }
        else
        {
            Destroy(gameObject); /// Nếu đã có một instance tồn tại, hủy GameObject này để đảm bảo chỉ có một instance duy nhất
        }


    }

    private void Start()
    {
        /// Khởi tạo hàng đợi để lưu trữ các đối tượng trong pool
        objectPool = new Queue<GameObject>();

        /// Khởi tạo pool
        CreateInitialPool();
    }

    public GameObject GetObject()
    {
        if (objectPool.Count == 0)
        {
            CreateNewObject(); /// Tạo thêm đối tượng nếu pool đã hết
        }

        GameObject obj = objectPool.Dequeue(); /// Lấy đối tượng từ pool
        obj.SetActive(true); /// Kích hoạt đối tượng trước khi trả về

        ///
        obj.transform.parent = null; /// Đặt đối tượng ra khỏi hierarchy của ObjectPool để nó có thể hoạt động độc lập trong scene

        return obj;

    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false); /// Vô hiệu hóa đối tượng trước khi trả về pool
        objectPool.Enqueue(obj); /// Thêm đối tượng trở lại pool
        obj.transform.parent = transform; /// Đặt đối tượng trở lại làm con của ObjectPool để tổ chức trong hierarchy
    }

    public void CreateInitialPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private void CreateNewObject()
    {
        GameObject obj = Instantiate(objectPrefab, transform);
        obj.SetActive(false); /// Vô hiệu hóa đối tượng sau khi tạo
        objectPool.Enqueue(obj); /// Thêm đối tượng vào pool
    }
}