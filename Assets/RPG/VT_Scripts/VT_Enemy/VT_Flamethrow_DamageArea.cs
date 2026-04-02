using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Flamethrow_DamageArea : MonoBehaviour
{
    private VT_Enemy_Boss enemy;

    private float damageCooldown;
    private float lastTimeDamaged;
    private int flameDamage;

    private void Awake()
    {
        enemy = GetComponentInParent<VT_Enemy_Boss>();
        damageCooldown = enemy.flameDamageCooldown;
        flameDamage = enemy.flameDamage;
    }

    private void OnTriggerStay(Collider other)
    {
        /// Hàm này gọi mỗi Frame khi có Collider nào va chạm với bản thân Object

        if (enemy.flamethrowActive == false)
        {
            return;
        }

        /// Áp dụng thời gian vào điều kiện để tính toán sát thương
        /// Dựa vào damageCooldown chứ không dựa vào Frame
        if (Time.time - lastTimeDamaged < damageCooldown)
        {
            return;
        }

        VT_IDamagable damagable = other.GetComponent<VT_IDamagable>();

        if (damagable != null)
        {
            damagable.TakeDamage(flameDamage);
            lastTimeDamaged = Time.time; /// Update the last time damage was applied

            /// Cập nhật flameDamageCooldown của enemy theo thời gian
            /// Trong trường hợp đòn tấn công của enemy bị Player dùng biện pháp 
            /// làm suy giảm sức mạnh của đòn tấn công (vd: ngăn chặn hoặc làm gián đoạn đòn tấn c)
            /// VD: flameDamageCooldown = 0.5 => damage được áp dụng mỗi 0.5s lên Player
            damageCooldown = enemy.flameDamageCooldown;
        }
    }
}
