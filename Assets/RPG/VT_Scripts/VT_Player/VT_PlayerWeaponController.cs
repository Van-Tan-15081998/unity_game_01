using System.Collections.Generic;
using UnityEngine;

public class VT_PlayerWeaponController : MonoBehaviour
{
    private VT_Player player;

    // Hằng số tham chiếu 
    private const float REFERENCE_BULLET_SPEED = 20;

    [SerializeField] private VT_Weapon currentWeapon; // Thông tin về vũ khí hiện tại của người chơi


    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefab; // Prefab của viên đạn
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2; // Số lượng vũ khí tối đa mà người chơi có thể mang theo
    [SerializeField] private List<VT_Weapon> weaponSlots;


    private void Start()
    {
        /// Lấy tham chiếu đến VT_Player để truy cập PlayerControls
        player = GetComponent<VT_Player>();

        ///
        AssignInputEvents();

        /// Trang bị vũ khí đầu tiên sau khi khởi tạo để đảm bảo rằng người chơi có vũ khí khi bắt đầu trò chơi
        Invoke("EquipStartingWeapon", .1f);
    }


    #region Slots Management - Pickup\Equip\Drop Weapons

    private void EquipStartingWeapon()
    {
        EquipWeapon(0); /// Trang bị vũ khí đầu tiên trong kho làm vũ khí hiện tại  
    }


    /// <summary>
    /// - Cài đặt vũ khí hiện tại. <br/>
    /// - Phát animation trang bị vũ khí mới khi thay đổi vũ khí. <br/>
    /// </summary>
    /// <param name="i"></param>
    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];

        ///


        ///
        player.weaponVisuals.PlayWeaponEquipAnimation();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newWeapon"></param>
    public void PickupWeapon(VT_Weapon newWeapon)
    {

        if ( weaponSlots.Count >= maxSlots)
        {
            return; /// Không thể nhặt vũ khí mới nếu đã đạt đến giới hạn
        }

        weaponSlots.Add(newWeapon); /// Thêm vũ khí mới vào kho

        ///
        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    /// <summary>
    /// 
    /// </summary>
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
        {
            return; /// Không thể bỏ vũ khí nếu chỉ có một vũ khí trong kho
        }

        weaponSlots.Remove(currentWeapon); /// Loại bỏ vũ khí hiện tại khỏi kho

        EquipWeapon(0); /// Chọn vũ khí tiếp theo trong kho làm vũ khí hiện tại
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    private void Shoot()
    {
        if (currentWeapon.CanShoot() == false)
        {
            return;
        }

        currentWeapon.bulletsInMagazine--; /// Giảm số lượng đạn khi bắn

        /// Tạo viên đạn mới tại điểm bắn với hướng bắn
        GameObject newBullet = VT_ObjectPool.instance.GetObject(); /// Lấy viên đạn từ Object Pool
        newBullet.transform.position = gunPoint.position; /// Đặt vị trí của viên đạn tại điểm bắn
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward); /// Đặt hướng của viên đạn theo hướng của điểm bắn

        /// Điều chỉnh tốc độ của viên đạn dựa trên thông số bulletSpeed của vũ khí hiện tại
        Rigidbody newBulletRigidbody = newBullet.GetComponent<Rigidbody>();

        newBulletRigidbody.mass = REFERENCE_BULLET_SPEED / bulletSpeed; /// Điều chỉnh khối lượng để đạt được tốc độ mong muốn
        newBulletRigidbody.velocity = BulletDirection() * bulletSpeed;

        /// Kích hoạt trigger "VT_Fire" trong Animator để phát animation bắn
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("VT_Fire");
        /// Or GetComponentInChildren<Animator>().SetTrigger("VT_Fire");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
        {
            direction.y = 0; // Loại bỏ thành phần y để viên đạn bay ngang
        }

        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim);

        return direction;
    }

    public bool HasOnlyOneWeapon()
    {
        return weaponSlots.Count <= 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public VT_Weapon CurrentWeapon()
    {
        return currentWeapon;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public VT_Weapon BackupWeapon()
    {
        foreach (VT_Weapon weapon in weaponSlots)
        {
            if (weapon != currentWeapon)
            {
                return weapon; /// Trả về vũ khí dự phòng (không phải vũ khí hiện tại)
            }
        }   

        return null; /// Trả về null nếu không có vũ khí dự phòng nào
    }

    public Transform GunPoint()
    {
        return gunPoint;
    }

    #region Input Events
    /// <summary>
    /// 
    /// </summary>
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        /// Đăng ký sự kiện bắn
        controls.VT_Character.Fire.performed += context => Shoot();

        /// Đăng ký sự kiện thay đổi vũ khí (ví dụ: nhấn phím 1, 2, 3 để chọn vũ khí)
        controls.VT_Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.VT_Character.EquipSlot2.performed += context => EquipWeapon(1);

        /// Đăng ký sự kiện bỏ vũ khí hiện tại
        controls.VT_Character.DropCurrentWeapon.performed += context => DropWeapon();

        /// Đăng ký sự kiện tái nạp đạn  
        controls.VT_Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload())
            {
                player.weaponVisuals.PlayReloadAnimation();
            }
        };
    }
    #endregion
}
