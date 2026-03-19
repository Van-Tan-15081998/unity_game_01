using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_PlayerWeaponController : MonoBehaviour
{
    private VT_Player player;

    // Hằng số tham chiếu 
    private const float REFERENCE_BULLET_SPEED = 20;

    [SerializeField] VT_WeaponData defaultWeaponData;


    [SerializeField] private VT_Weapon currentWeapon; /// Thông tin về vũ khí hiện tại của người chơi
    private bool weaponReady;
    private bool isShooting;


    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefab; /// Prefab của viên đạn
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2; /// Số lượng vũ khí tối đa mà người chơi có thể mang theo
    [SerializeField] private List<VT_Weapon> weaponSlots;

    [SerializeField] private GameObject weaponPickupPrefab;


    private void Start()
    {
        /// Lấy tham chiếu đến VT_Player để truy cập PlayerControls
        player = GetComponent<VT_Player>();

        ///
        AssignInputEvents();

        /// Trang bị vũ khí đầu tiên sau khi khởi tạo để đảm bảo rằng người chơi có vũ khí khi bắt đầu trò chơi
        Invoke("EquipStartingWeapon", .1f);
    }

    private void Update()
    {
        if (isShooting)
        {
            Shoot(); /// Thực hiện bắn

        }
    }


    #region Slots Management - Pickup\Equip\Drop\Ready Weapons

    private void EquipStartingWeapon()
    {
        weaponSlots[0] = new VT_Weapon(defaultWeaponData);

        EquipWeapon(0); /// Trang bị vũ khí đầu tiên trong kho làm vũ khí hiện tại  
    }


    /// <summary>
    /// - Cài đặt vũ khí hiện tại. <br/>
    /// - Phát animation trang bị vũ khí mới khi thay đổi vũ khí. <br/>
    /// </summary>
    /// <param name="i"></param>
    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count)
        {
            return;
        }

        /// Đặt trạng thái vũ khí không sẵn sàng trong quá trình trang bị để ngăn người chơi bắn 
        /// hoặc thực hiện các hành động khác với vũ khí trong khi nó đang được trang bị.
        SetWeaponReady(false);

        ///
        currentWeapon = weaponSlots[i];

        ///
        player.weaponVisuals.PlayWeaponEquipAnimation();

        ///
        VT_CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newWeapon"></param>
    public void PickupWeapon(VT_Weapon newWeapon)
    {
        /// Nếu vũ khí đã có trong trang bị thì cộng thêm số lượng đạn
        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;

            return;
        }

        /// Nếu đã hết Slot => thay thế vũ khí hiện tại bằng vũ khí mới (Khác loại)
        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;

            CreateWeaponOnTheGround();
            EquipWeapon(weaponIndex);

            return;
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

        CreateWeaponOnTheGround();

        ///
        weaponSlots.Remove(currentWeapon); /// Loại bỏ vũ khí hiện tại khỏi kho

        EquipWeapon(0); /// Chọn vũ khí tiếp theo trong kho làm vũ khí hiện tại
    }

    private void CreateWeaponOnTheGround()
    {
        /// Cài đặt vị trí bỏ vũ khí
        GameObject droppedWeapon = VT_ObjectPool.instance.GetObject(weaponPickupPrefab);
        droppedWeapon.GetComponent<VT_PickupWeapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready)
    {
        weaponReady = ready;
    }

    public bool WeaponReady()
    {
        return weaponReady;
    }
    #endregion

    private IEnumerator BrustFire()
    {
        /// Vào trạng thái bắn Loạt thì cài đặt trạng thái của súng để tránh bắn thực hiện bắn Loạt liên tục
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            /// Sau khi thực hiện phát bắn Loạt thì cài đặt lại trạng thái sẵn sàng của súng
            if (i >= currentWeapon.bulletsPerShot)
            {
                SetWeaponReady(true);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Shoot()
    {
        ///
        if (WeaponReady() == false)
        {
            return; /// Không thể bắn nếu vũ khí không sẵn sàng
        }

        ///
        if (currentWeapon.CanShoot() == false)
        {
            return; /// Không thể bắn nếu vũ khí không đủ đạn hoặc chưa sẵn sàng để bắn dựa trên fireRate
        }

        /// Phát animation bắn
        player.weaponVisuals.PlayFireAnimation();

        ///
        if (currentWeapon.shootType == VT_ShootType.Single)
        {
            isShooting = false; /// Đối với kiểu bắn đơn, sau khi bắn một viên đạn, đặt isShooting thành false để ngăn bắn liên tục
        }

        ///
        if (currentWeapon.BurstActivated() == true)
        {
            StartCoroutine(BrustFire());
            return;
        }

        ///
        FireSingleBullet();


    }

    private void FireSingleBullet()
    {
        /// Giảm số lượng đạn khi bắn
        currentWeapon.bulletsInMagazine--;

        /// Tạo viên đạn mới tại điểm bắn với hướng bắn
        GameObject newBullet = VT_ObjectPool.instance.GetObject(bulletPrefab); /// Lấy viên đạn từ Object Pool
        newBullet.transform.position = GunPoint().position; /// Đặt vị trí của viên đạn tại điểm bắn
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward); /// Đặt hướng của viên đạn theo hướng của điểm bắn

        /// Điều chỉnh tốc độ của viên đạn dựa trên thông số bulletSpeed của vũ khí hiện tại
        Rigidbody newBulletRigidbody = newBullet.GetComponent<Rigidbody>();

        /// Cài đặt khoảng cách bay tối đa của viên đạn dựa vào loại vũ khí
        VT_Bullet bulletScript = newBullet.GetComponent<VT_Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance);

        /// Áp dụng độ giật (spread) vào hướng bắn để tạo ra sự không chính xác khi bắn
        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        newBulletRigidbody.mass = REFERENCE_BULLET_SPEED / bulletSpeed; /// Điều chỉnh khối lượng để đạt được tốc độ mong muốn
        newBulletRigidbody.velocity = bulletsDirection * bulletSpeed;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Reload()
    {
        /// Đặt trạng thái vũ khí không sẵn sàng trong quá trình tái nạp để ngăn người chơi bắn 
        /// hoặc thực hiện các hành động khác với vũ khí trong khi nó đang được tái nạp.
        SetWeaponReady(false);

        player.weaponVisuals.PlayReloadAnimation();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
        {
            direction.y = 0; // Loại bỏ thành phần y để viên đạn bay ngang
        }

        return direction;
    }

    public bool HasOnlyOneWeapon()
    {
        return weaponSlots.Count <= 1;
    }

    public VT_Weapon WeaponInSlots(VT_WeaponType weaponType)
    {
        foreach(VT_Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
            {
                return weapon;
            }
        }

        return null;
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
    public Transform GunPoint()
    {
        return player.weaponVisuals.CurrentWeaponModel().gunPoint;
    }

    #region Input Events
    /// <summary>
    /// 
    /// </summary>
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        /// Đăng ký sự kiện bắn
        controls.VT_Character.Fire.performed += context => isShooting = true; /// Khi nút bắn được nhấn, đặt isShooting thành true để bắt đầu bắn
        controls.VT_Character.Fire.canceled += context => isShooting = false; /// Khi nút bắn được thả, đặt isShooting thành false để dừng bắn

        /// Đăng ký sự kiện thay đổi vũ khí (ví dụ: nhấn phím 1, 2, 3 để chọn vũ khí)
        controls.VT_Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.VT_Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.VT_Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.VT_Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.VT_Character.EquipSlot5.performed += context => EquipWeapon(4);

        /// Đăng ký sự kiện bỏ vũ khí hiện tại
        controls.VT_Character.DropCurrentWeapon.performed += context => DropWeapon();

        /// Đăng ký sự kiện tái nạp đạn  
        controls.VT_Character.Reload.performed += context =>
        {
            /// Chỉ cho phép tái nạp nếu:
            /// - vũ khí hiện tại có thể tái nạp
            /// - vũ khí đang ở trạng thái sẵn sàng
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        /// Chuyển đổi chế độ bắn của súng
        controls.VT_Character.ToogleWeaponMode.performed += context => currentWeapon.ToggleBurst();
    }
    #endregion
}
