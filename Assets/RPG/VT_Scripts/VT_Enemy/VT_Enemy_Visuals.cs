using System.Collections.Generic;
using UnityEngine;

public enum VT_Enemy_MeleeWeaponType { OneHand, Throw, Unarmed }
public enum VT_Enemy_RangeWeaponType { Pistol, Revolver, Shotgun, AutoRifle, Rifle }

public class VT_Enemy_Visuals : MonoBehaviour
{
    public GameObject currentWeaponModel { get; private set; }

    [Header("Corruption Visuals")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        
    }

    public void EnableWeaponTrail(bool enable)
    {
        VT_Enemy_WeaponModel currentWeaponScript = currentWeaponModel.GetComponent<VT_Enemy_WeaponModel>();

        currentWeaponScript.EnableTrailEffect(enable);
    }

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomCorruption()
    {
        List<int> avalibleIndexs = new List<int>();
        corruptionCrystals = CollectCorruptionCrystals();

        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            avalibleIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for (int i = 0; i < corruptionAmount; i++)
        {
            if (avalibleIndexs.Count == 0)
            {
                break;
            }

            int randomIndex = Random.Range(0, avalibleIndexs.Count);
            int objectIndex = avalibleIndexs[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            avalibleIndexs.RemoveAt(randomIndex);
        }
    }

    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<VT_Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<VT_Enemy_Range>() != null;

        if (thisEnemyIsMelee)
        {
            currentWeaponModel = FindMeleeWeaponModel();
        }
        if (thisEnemyIsRange)
        {
            currentWeaponModel = FindRangeWeaponModel();
        }

        currentWeaponModel.SetActive(true);

        ///
        OverrideAnimatorControllerIfCan();
    }

    private GameObject FindMeleeWeaponModel()
    {
        VT_Enemy_WeaponModel[] weaponModels = GetComponentsInChildren<VT_Enemy_WeaponModel>(true);

        VT_Enemy_MeleeWeaponType weaponType = GetComponent<VT_Enemy_Melee>().weaponType;

        List<VT_Enemy_WeaponModel> filteredWeaponModel = new List<VT_Enemy_WeaponModel>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                filteredWeaponModel.Add(weaponModel);
            }
        }

        int randomIndex = Random.Range(0, filteredWeaponModel.Count);
        return filteredWeaponModel[randomIndex].gameObject;
    }

    private GameObject FindRangeWeaponModel()
    {
        VT_Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<VT_Enemy_RangeWeaponModel>(true);

        VT_Enemy_RangeWeaponType weaponType = GetComponent<VT_Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                return weaponModel.gameObject;
            }
        }

        return null;    
    }

    private void OverrideAnimatorControllerIfCan()
    {
        /// Nếu Unarmed => Sử dụng AnimatorOverrideController
        AnimatorOverrideController overrideController =
                    currentWeaponModel.GetComponent<VT_Enemy_WeaponModel>()?.overrideController;

        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMaterial = new Material(skinnedMeshRenderer.material);

        newMaterial.mainTexture = colorTextures[randomIndex];

        skinnedMeshRenderer.material = newMaterial;
    }

    private GameObject[] CollectCorruptionCrystals()
    {
        VT_Enemy_CorruptionCrystal[] crystalComponents = GetComponentsInChildren<VT_Enemy_CorruptionCrystal>(true);
        GameObject[] corruptionCrystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            corruptionCrystals[i] = crystalComponents[i].gameObject;
        }

        return corruptionCrystals;
    }

}
