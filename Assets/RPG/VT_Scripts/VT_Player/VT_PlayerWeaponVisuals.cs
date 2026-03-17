using UnityEngine;
using UnityEngine.Animations.Rigging;

public class VT_PlayerWeaponVisuals : MonoBehaviour
{
    private VT_Player player;

    private Animator anim;
    private bool isGrabbingWeapon; // Biến để theo dõi trạng thái đang thực hiện animation lấy vũ khí

    #region Gun Transforms region
    //[SerializeField] private Transform[] gunTransforms;

    //[SerializeField] private Transform pistol;
    //[SerializeField] private Transform revolver;
    //[SerializeField] private Transform autoRifle;
    //[SerializeField] private Transform shortgun;
    //[SerializeField] private Transform rifle;

    //private Transform currentGun;
    #endregion

    [SerializeField] private VT_WeaponModel[] weaponModels; /// Mảng chứa thông tin về các loại vũ khí (loại, vị trí IK tay trái, kiểu cầm)
    [SerializeField] private VT_BackupWeaponModel[] backupWeaponModels;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncrease_RigWeight;
    private Rig rig;

    [Header("Left Hand IK")]
    [SerializeField] private float leftHandIkWeightIncreaseRate;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    private bool shouldIncrease_LeftHandIKWeight;

    private void Start()
    {
        player = GetComponentInChildren<VT_Player>();

        // anim = GetComponentInParent<Animator>();
        // Di chuyển Script này (VT_WeaponVisualController) từ Weapon_Holder (Đang cố định tại RightHand) => 
        // => Di chuyển lên Player để dễ dàng truy cập thay vì truy cập quá sâu vào RightHand (Hirearchy)
        // => Do đó, cần thay đổi cách lấy Animator từ GetComponentInParent sang GetComponentInChildren
        // Vì Animator nằm trên character_main, nên sử dụng GetComponentInChildren để lấy Animator
        anim = GetComponentInChildren<Animator>();

        // Lấy Rig component từ Player (nếu có)
        rig = GetComponentInChildren<Rig>();

        weaponModels = GetComponentsInChildren<VT_WeaponModel>(true);
        // true để bao gồm cả các GameObject con bị tắt (inactive) trong việc lấy VT_WeaponModel

        // Mặc định kích hoạt súng đầu tiên (pistol)
        //SwitchOn(pistol);

        ///
        backupWeaponModels = GetComponentsInChildren<VT_BackupWeaponModel>(true);   
    }

    private void Update()
    {
        //CheckWeaponSwitch();

        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }
   
    /// <summary>
    /// TODO: Phương thức để lấy thông tin về vũ khí hiện tại của player
    /// </summary>
    public VT_WeaponModel CurrentWeaponModel()
    {
        VT_WeaponModel weaponModel = null;

        VT_WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
                weaponModel = weaponModels[i];
        }

        return weaponModel;
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    public void PlayReloadAnimation()
    {
        if (isGrabbingWeapon)
        {
            return; /// Không phát animation reload nếu đang thực hiện animation lấy vũ khí
        }

        float reloadSpeed = player.weapon.CurrentWeapon().reloadSpeed;

        anim.SetTrigger("VT_Reload");
        anim.SetFloat("VT_ReloadSpeed", reloadSpeed);

        /// Tắt Rig khi reload để tránh xung đột với animation reload
        ReduceRigWeight();
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().equipAnimationType;

        float equipmentSpeed = player.weapon.CurrentWeapon().equipmentSpeed;

        leftHandIK.weight = 0; /// Tắt IK tay trái để tránh xung đột với animation cầm vũ khí mới

        /// Tạm dừng Rig để tránh xung đột với animation cầm vũ khí mới
        ReduceRigWeight();

        /// Thiết lập tham số VT_WeaponGrabType trong Animator để xác định loại cầm vũ khí
        /// Ví dụ: 0 cho SideGrab, 1 cho BackGrab (được định nghĩa trong enum GrabType)
        anim.SetFloat("VT_EquipType", ((float)equipType));
        anim.SetTrigger("VT_EquipWeapon");
        anim.SetFloat("VT_EquipSpeed", equipmentSpeed);

        SetBusyEquipingWeaponTo(true);
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    public void SetBusyEquipingWeaponTo(bool busy)
    {
        isGrabbingWeapon = busy;
        anim.SetBool("VT_BusyEquipingWeapon", isGrabbingWeapon);
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    public void SwitchOnCurrentWeaponModel()
    {
        /// animationLayerIndex = 1 => VT_Common_Weapon_Layer (Animator layer dành cho các loại súng có kiểu cầm CommonHold)
        /// animationLayerIndex = 2 => VT_Shortgun_Weapon_Layer (Animator layer dành cho các loại súng có kiểu cầm LowHold)
        /// animationLayerIndex = 3 => VT_Rifle_Weapon_Layer (Animator layer dành cho các loại súng có kiểu cầm HighHold)
        int animationLayerIndex = (int)CurrentWeaponModel().holdType;

        SwitchOffWeaponModels();

        SwitchOffBackupWeaponModels();

        /// Nếu player có nhiều hơn 1 vũ khí (có vũ khí dự phòng), thì bật model vũ khí dự phòng để hiển thị ở lưng
        if (player.weapon.HasOnlyOneWeapon() == false)
        {
            SwitchOnBackupWeaponModel();
        }
        
        SwitchAnimationLayer(animationLayerIndex);

        CurrentWeaponModel().gameObject.SetActive(true);

        /// Cập nhật vị trí và xoay của tay trái theo súng mới
        AttachLeftHand();
    }

    /// <summary>
    /// TODO: Tắt tất cả các model vũ khí trước khi bật model vũ khí mới để tránh việc nhiều model vũ khí cùng hiển thị một lúc
    /// </summary>
    public void SwitchOffWeaponModels()
    {
        foreach (VT_WeaponModel gun in weaponModels)
        {
            // Tắt tất cả các loại súng
            gun.gameObject.SetActive(false);
        }
    }

    public void SwitchOffBackupWeaponModels()
    {
        foreach (VT_BackupWeaponModel gun in backupWeaponModels)
        {
            // Tắt tất cả các loại súng
            gun.gameObject.SetActive(false);
        }
    }


    public void SwitchOnBackupWeaponModel()
    {
       VT_WeaponType weaponType = player.weapon.BackupWeapon().weaponType;

        foreach (VT_BackupWeaponModel backupModel in backupWeaponModels)
        {
            if (backupModel.weaponType == weaponType)
            {
                backupModel.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    private void SwitchAnimationLayer(int layerIndex)
    {
        // Tắt tất cả các layer animation
        for (int i = 0; i < anim.layerCount; i++)
        {
            // Đặt trọng số của tất cả các layer animation về 0 (tắt)
            anim.SetLayerWeight(i, 0);
        }
        // Kích hoạt layer animation được chọn
        anim.SetLayerWeight(layerIndex, 1);
    }

    #region Animation Rigging Methods

    /// <summary>
    /// TODO: 
    /// </summary>
    private void AttachLeftHand()
    {
        //Transform targetTransform = currentGun.GetComponentInChildren<VT_LeftHandTargetTransform>().transform;
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIkWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
            {
                shouldIncrease_LeftHandIKWeight = false; // Dừng tăng khi đạt trọng số tối đa
            }
        }
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;
            if (rig.weight >= 1)
            {
                shouldIncrease_RigWeight = false; // Dừng tăng khi đạt trọng số tối đa
            }
        }
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    private void ReduceRigWeight()
    {
        rig.weight = .15f;
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    public void MaximizeRigWeight()
    {
        shouldIncrease_RigWeight = true;
    }

    /// <summary>
    /// TODO:
    /// </summary>
    public void MaximizeLeftHandWeight()
    {
        shouldIncrease_LeftHandIKWeight = true;
    }

    #endregion
}


