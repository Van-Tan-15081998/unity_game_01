using UnityEngine;

public enum VT_WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle,
    // Thêm các loại vũ khí khác nếu cần thiết
}

public enum VT_ShootType
{
    Single,
    Auto
}

// Không cần kế thừa MonoBehaviour vì đây chỉ là một class để lưu trữ thông tin về vũ khí,
// không cần phải gắn vào GameObject nào trong Unity.
// Việc này giúp cải thiện hiệu suất của game.

[System.Serializable] /// Cho phép hiển thị trong Inspector của Unity nếu cần thiết.

public class VT_Weapon
{
    public VT_WeaponType weaponType; /// Loại vũ khí (Pistol, Revolver, AutoRifle, Shotgun, Rifle).

    #region Regular mode variables
    [Header("Shooting specifics")]
    public VT_ShootType shootType; /// Kiểu bắn của vũ khí (Single, Auto, ...).
    private float defaultFireRate = 1; /// Tốc độ bắn của vũ khí
    public float fireRate = 1; /// Tốc độ bắn của vũ khí
    private float lastShootTime; /// Thời gian lần cuối cùng vũ khí được bắn.
    #endregion

    [Header("Magazine details")]
    public int bulletsInMagazine; /// Số lượng đạn hiện tại của vũ khí.
    public int magazineCapacity; /// Số lượng đạn tối đa mà một băng đạn có thể chứa.
    public int totalReserveAmmo; /// Số lượng đạn dự trữ tối đa mà người chơi có thể mang theo cho loại vũ khí này.


    #region Weapon generic info variables
    public float reloadSpeed { get; private set; } /// Thời gian cần thiết để nạp lại vũ khí
    public float equipmentSpeed { get; private set; } /// Thời gian cần thiết để trang bị vũ khí
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    #endregion

    #region Burst mode  variables
    [Header("Burst fire")]
    public int bulletsPerShot; /// Số lượng viên đạn trong một Loạt bắn
                               /// => Có thể dùng cho nhiều loại súng Không chỉ trong Burst mode và Shortgun

    private bool burstAvalible;
    public bool burstActive;

    private int burstBulletsPerShot;
    private float burstFireRate;
    public float burstFireDelay { get; private set; } /// Khoảng cách thời gian giữa các viên đạn đơn trong 1 Loạt bắn
    #endregion

    #region Weapon spread variables
    [Header("Spread")]
    /// Độ giật của vũ khí, ảnh hưởng đến độ chính xác của viên đạn khi bắn. Giá trị này có thể được sử dụng để tính toán sự phân tán của viên đạn khi bắn, 
    /// tạo ra hiệu ứng giật và làm cho việc ngắm bắn trở nên khó khăn hơn khi bắn liên tục.
    private float currentSpread = 2;

    private float baseSpread = 1;
    private float maximumSpread = 3;
    private float spreadIncreaseRate = .15f;

    private float lastSpreadUpdateTime; /// Thời gian lần cuối cập nhật Spread
    private float spreadCooldown = 1;
    #endregion

    public VT_Weapon(VT_WeaponData weaponData)
    {
        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;


        burstAvalible = weaponData.burstAvalible;
        burstActive = weaponData.burstActive;
        burstBulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;


        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;


        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;



        defaultFireRate = fireRate;
    }


    #region Spread methods
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        /// Tạo một giá trị ngẫu nhiên trong khoảng từ -spreadAmount đến +spreadAmount.
        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        /// Tạo một Quaternion đại diện cho sự xoay ngẫu nhiên dựa trên giá trị ngẫu nhiên đã tạo.
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        /// Áp dụng sự xoay ngẫu nhiên vào hướng ban đầu của viên đạn để tạo ra hiệu ứng giật.
        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        /// Reset currentSpread
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        /// 
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);

    }
    #endregion

    #region Burst methods

    public bool BurstActivated()
    {
        /// Shortgun luôn luôn có chế độ bắn Burst
        if (weaponType == VT_WeaponType.Shotgun)
        {
            burstFireDelay = 0; /// Các viên đạn rời khỏi nòng súng ngay lập tức mà không có Delay
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (burstAvalible == false)
        {
            return;
        }

        burstActive = !burstActive;

        if (burstAvalible)
        {
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion


    public bool CanShoot()
    {
        return HaveEnoughBullets() && ReadyToFire();
    }

    private bool ReadyToFire()
    {
        /// Kiểm tra xem đã đủ thời gian kể từ lần bắn cuối cùng dựa trên fireRate hay chưa.
        /// Ví dụ: Nếu fireRate là 2 viên/giây, thì thời gian giữa các lần bắn phải là 0.5 giây (1/fireRate).
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time; /// Cập nhật thời gian lần bắn cuối cùng
            return true; /// Trả về true nếu vũ khí đã sẵn sàng để bắn.
        }

        return false;
    }

    #region Reload methods
    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
        {
            return false; // Không thể nạp nếu băng đạn đã đầy.
        }

        if (totalReserveAmmo > 0)
        {
            return true; // Trả về true nếu có thể nạp đạn.
        }

        return false;
    }

    public void RefillBullets()
    {
        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo; // Chỉ nạp đạn bằng số lượng đạn dự trữ còn lại nếu nó ít hơn sức chứa của băng đạn.
        }

        totalReserveAmmo -= bulletsToReload; // Giảm số lượng đạn dự trữ sau khi nạp.
        bulletsInMagazine = bulletsToReload; // Cập nhật số lượng đạn trong băng đạn sau khi nạp.

        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0; // Đảm bảo số lượng đạn dự trữ không âm.
        }
    }

    private bool HaveEnoughBullets()
    {
        if (bulletsInMagazine > 0)
        {
            return true; /// Trả về true nếu có thể bắn.
        }

        return false; /// Trả về false nếu không còn đạn.
    }
    #endregion
}
