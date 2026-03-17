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


// Không cần kế thừa MonoBehaviour vì đây chỉ là một class để lưu trữ thông tin về vũ khí,
// không cần phải gắn vào GameObject nào trong Unity.
// Việc này giúp cải thiện hiệu suất của game.

[System.Serializable] // Cho phép hiển thị trong Inspector của Unity nếu cần thiết.

public class VT_Weapon
{
    public VT_WeaponType weaponType; // Loại vũ khí (Pistol, Revolver, AutoRifle, Shotgun, Rifle).

    public int bulletsInMagazine; // Số lượng đạn hiện tại của vũ khí.
    public int magazineCapacity; // Số lượng đạn tối đa mà một băng đạn có thể chứa.
    public int totalReserveAmmo; // Số lượng đạn dự trữ tối đa mà người chơi có thể mang theo cho loại vũ khí này.

    [Range(1,3)]
    public float reloadSpeed = 1; // Thời gian cần thiết để nạp lại vũ khí
    [Range(1,3)]
    public float equipmentSpeed = 1; // Thời gian cần thiết để trang bị vũ khí

    public bool CanShoot()
    {
        return HaveEnoughBullets();
    }

    private bool HaveEnoughBullets()
    {
        if (bulletsInMagazine > 0)
        {
            return true; // Trả về true nếu có thể bắn.
        }

        return false; // Trả về false nếu không còn đạn.
    }

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
}
