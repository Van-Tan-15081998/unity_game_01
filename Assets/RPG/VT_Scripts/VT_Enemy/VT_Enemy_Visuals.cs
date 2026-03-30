using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum VT_Enemy_MeleeWeaponType { OneHand, Throw, Unarmed }
public enum VT_Enemy_RangeWeaponType { Pistol, Revolver, Shotgun, AutoRifle, Rifle }

public class VT_Enemy_Visuals : MonoBehaviour
{
    public GameObject currentWeaponModel { get; private set; }

    public GameObject grenadeModel;

    [Header("Corruption Visuals")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Rig references")]
    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private MultiAimConstraint weaponAimConstraint;

    private float leftHandTargetWeight;
    private float weaponAimTargetWeight;
    private float rigChangeRate;

    private void Update()
    {
        if (leftHandIKConstraint != null)
        {
            leftHandIKConstraint.weight = AdjustIKWeight(leftHandIKConstraint.weight, leftHandTargetWeight);
        }

        if (weaponAimConstraint != null)
        {
            weaponAimConstraint.weight = AdjustIKWeight(weaponAimConstraint.weight, weaponAimTargetWeight);
        }
    }

    private void Awake()
    {

    }

    public void EnableGrenadeModel(bool active)
    {
        grenadeModel.SetActive(active);
    }

    public void EnableWeaponModel(bool active)
    {
        currentWeaponModel?.gameObject.SetActive(active);
    }

    public void EnableWeaponTrail(bool enable)
    {
        VT_Enemy_WeaponModel currentWeaponScript = currentWeaponModel.GetComponent<VT_Enemy_WeaponModel>();

        currentWeaponScript.EnableTrailEffect(enable);
    }

    public void EnableSecondaryWeaponModel(bool active)
    {
        FindSecondaryWeaponModel()?.SetActive(active);
    }

    public void SetupLook()
    {
        SetupDefaultIK();
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
                SwitchAnimationLayer((int)weaponModel.weaponHoldType);
                SetupLeftHandIK(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                return weaponModel.gameObject;
            }
        }

        return null;
    }

    private GameObject FindSecondaryWeaponModel()
    {
        VT_Enemy_SecondaryRangeWeaponModel[] weaponModels = GetComponentsInChildren<VT_Enemy_SecondaryRangeWeaponModel>(true);

        VT_Enemy_RangeWeaponType weaponType = GetComponentInParent<VT_Enemy_Range>().weaponType;

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

    private void SwitchAnimationLayer(int layerIndex)
    {
        Animator anim = GetComponentInChildren<Animator>();

        // Tắt tất cả các layer animation
        for (int i = 0; i < anim.layerCount; i++)
        {
            // Đặt trọng số của tất cả các layer animation về 0 (tắt)
            anim.SetLayerWeight(i, 0);
        }
        // Kích hoạt layer animation được chọn
        anim.SetLayerWeight(layerIndex, 1);
    }

    /// <summary>
    /// Cài đặt mặc định cho chỉ số Weight của Rig (Tự thực hiện - Ngoài hướng dẫn)
    /// </summary>
    private void SetupDefaultIK()
    {
        /// Enemy Range có Rig Component (leftHandIKConstraint, weaponAimConstraint)
        /// Enemy Melee không có Rig Component nên cần kiểm tra
        if (leftHandIKConstraint != null && weaponAimConstraint != null)
        {
            EnableIK(false, false);
        }

    }

    public void EnableIK(bool enableLeftHand, bool enableAim, float changeRate = 10)
    {
        /// [changeRate] có ý nghĩa là sự chuyển đổi giá trị có diễn ra nhanh hay chậm hoặc là nhanh đến mức nào
        rigChangeRate = changeRate;

        leftHandTargetWeight = enableLeftHand ? 1 : 0;
        weaponAimTargetWeight = enableAim ? 1 : 0;
    }

    private void SetupLeftHandIK(Transform leftHandTarget, Transform leftElbowTarget)
    {
        leftHandIK.localPosition = leftHandTarget.localPosition;
        leftHandIK.localRotation = leftHandTarget.localRotation;

        leftElbowIK.localPosition = leftElbowTarget.localPosition;
        leftElbowIK.localRotation = leftElbowTarget.localRotation;
    }

    private float AdjustIKWeight(float currentWeight, float targetWeight)
    {
        /// Để sự chuyển đổi IK giữa 2 animation trở nên mượt mà thì chỉ số WEIGHT nên thay đổi
        /// nhanh dần đều hoặc chậm dần đều (vd: 1 => 0.9 => 0.85 => ...0) chứ không nên thay đổi đột ngột (vd: 1 => 0)

        if (Mathf.Abs(currentWeight - targetWeight) > 0.05f)
        {
            return Mathf.Lerp(currentWeight, targetWeight, rigChangeRate * Time.deltaTime);
        }

        return targetWeight;
    }
}
