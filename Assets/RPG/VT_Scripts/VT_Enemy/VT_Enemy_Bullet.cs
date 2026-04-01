using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_Bullet : VT_Bullet /// Không sử dụng nữa
{
    protected override void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX();
        ReturnBulletToPool();

        VT_Player player = collision.gameObject.GetComponentInParent<VT_Player>();
    }
}
