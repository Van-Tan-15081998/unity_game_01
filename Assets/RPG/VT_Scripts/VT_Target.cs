using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Thêm component Rigidbody để đảm bảo rằng đối tượng có thể tương tác vật lý và bị ảnh hưởng bởi lực, va chạm, v.v.
[RequireComponent(typeof(Rigidbody))]
public class VT_Target : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("VT_Enemy");
    }
}
