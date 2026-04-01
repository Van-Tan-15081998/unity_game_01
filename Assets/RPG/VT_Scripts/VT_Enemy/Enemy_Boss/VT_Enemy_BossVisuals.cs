using UnityEngine;

public class VT_Enemy_BossVisuals : MonoBehaviour
{
    private VT_Enemy_Boss enemy;

    /// Từ center của landing + landingOffset để có vị trí center cuối cùng cho động tác Jump với Hummer
    /// => Nhằm giúp vị trí của Búa đạp ngay trung tâm của LandingZone
    [SerializeField] private float landingOffset = 1f;

    [SerializeField] private ParticleSystem landingZoneFX;
    [SerializeField] private GameObject[] weaponTrails;

    [Header("Batteries")]
    [SerializeField] private GameObject[] batteries;
    [SerializeField] private float initalBatteryScaleY = .2f;

    private float dischargeSpeed;
    private float rechargeSpeed;

    private bool isRecharging;

    private void Awake()
    {
        enemy = GetComponent<VT_Enemy_Boss>();

        landingZoneFX.transform.parent = null;
        landingZoneFX.Stop();

        ResetBatteries();
    }

    private void Update()
    {
        UpdateBatteriesScale();
    }

    public void EnableWeaponTrail(bool active)
    {
        if (weaponTrails.Length <= 0)
        {
            return;
        }

        foreach (var weaponTrail in weaponTrails)
        {
            weaponTrail.gameObject.SetActive(active);
        }
    }

    public void PlaceLandingZone(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 offset = direction.normalized * landingOffset;

        if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {
            landingZoneFX.transform.position = target + offset;
        }
        else
        {
            landingZoneFX.transform.position = target;
        }


        landingZoneFX.Clear();

        var mainModule = landingZoneFX.main;
        mainModule.startLifetime = enemy.travelTimeToTarget * 2;

        landingZoneFX.Play();
    }

    private void UpdateBatteriesScale()
    {
        if (batteries.Length <= 0)
        {
            return;
        }

        foreach (GameObject battery in batteries)
        {
            if (battery.activeSelf)
            {
                float scaleChange = (isRecharging ? rechargeSpeed : -dischargeSpeed) * Time.deltaTime;

                float newScaleY = Mathf.Clamp(
                    battery.transform.localScale.y + scaleChange,
                    0,
                    initalBatteryScaleY);

                battery.transform.localScale = new Vector3(0.15f, newScaleY, 0.15f);

                if (battery.transform.localScale.y <= 0)
                {
                    battery.SetActive(false);
                }
            }
        }
    }

    public void ResetBatteries()
    {
        isRecharging = true;

        rechargeSpeed = initalBatteryScaleY / enemy.abilityCooldown;
        dischargeSpeed = initalBatteryScaleY / (enemy.flameThrowDuration * .75f);

        foreach (GameObject battery in batteries)
        {
            battery.SetActive(true);
        }
    }

    public void DischargeBatteries()
    {
        isRecharging = false;
    }
}
