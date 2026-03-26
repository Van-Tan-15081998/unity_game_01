using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_Enemy_WeaponModel : MonoBehaviour
{
    public VT_Enemy_MeleeWeaponType weaponType;

    public AnimatorOverrideController overrideController;

    public VT_Enemy_MeleeWeaponData weaponData;

    [SerializeField] private GameObject[] trailEffects;

    private void Awake()
    {
        //EnableTrailEffect(false); /// Không cần nếu Inspector đã Set Unactive
    }

    public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }
}
