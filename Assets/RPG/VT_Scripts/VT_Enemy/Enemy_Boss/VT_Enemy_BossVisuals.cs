using UnityEngine;

public class VT_Enemy_BossVisuals : MonoBehaviour
{
    private VT_Enemy_Boss enemy;

    [SerializeField] private ParticleSystem landingZoneFX;

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

    public void PlaceLandingZone(Vector3 target)
    {
        landingZoneFX.transform.position = target;
        
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
