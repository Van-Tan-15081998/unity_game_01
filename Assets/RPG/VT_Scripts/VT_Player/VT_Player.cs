using UnityEngine;

public class VT_Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls { get; private set; } // Read-Only
    public VT_Player_AimController aim { get; private set; } // Read-Only

    public VT_Player_Movement movement { get; private set; } // Read-Only

    public VT_Player_WeaponController weapon { get; private set; } // Read-Only

    public VT_Player_WeaponVisuals weaponVisuals { get; private set; } // Read-Only

    public VT_Player_Interaction interaction { get; private set; } // Read-Only

    public VT_Player_Health health { get; private set; }

    public VT_Ragdoll ragdoll { get; private set; }

    public Animator anim { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
        aim = GetComponent<VT_Player_AimController>();
        movement = GetComponent<VT_Player_Movement>();   
        weapon = GetComponent<VT_Player_WeaponController>();
        weaponVisuals = GetComponent<VT_Player_WeaponVisuals>();
        interaction = GetComponent<VT_Player_Interaction>();

        health = GetComponent<VT_Player_Health>();
        ragdoll = GetComponent<VT_Ragdoll>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {

        controls.Disable();
    }
}
