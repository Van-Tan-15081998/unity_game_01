using UnityEngine;

public enum HangType { LowBackHang, BackHang, SideHang }

public class VT_BackupWeaponModel : MonoBehaviour
{
    public VT_WeaponType weaponType;

    [SerializeField] private HangType hangType;

    private void Start()
    {

    }

    public void Activate(bool activated)
    {
        gameObject.SetActive(activated);
    }

    public bool HangTypeIs(HangType hangType)
    {
        return this.hangType == hangType;
    }
}
