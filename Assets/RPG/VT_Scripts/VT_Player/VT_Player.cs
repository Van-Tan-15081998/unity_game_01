using UnityEngine;

public class VT_Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls { get; private set; } // Read-Only
    public VT_PlayerAim aim { get; private set; } // Read-Only

    public VT_PlayerMovement movement { get; private set; } // Read-Only

    public VT_PlayerWeaponController weapon { get; private set; } // Read-Only

    public VT_PlayerWeaponVisuals weaponVisuals { get; private set; } // Read-Only

    public VT_PlayerInteraction interaction { get; private set; } // Read-Only

    public VT_PlayerHealth health { get; private set; }

    public VT_Ragdoll ragdoll { get; private set; }

    public Animator anim { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
        aim = GetComponent<VT_PlayerAim>();
        movement = GetComponent<VT_PlayerMovement>();   
        weapon = GetComponent<VT_PlayerWeaponController>();
        weaponVisuals = GetComponent<VT_PlayerWeaponVisuals>();
        interaction = GetComponent<VT_PlayerInteraction>();

        health = GetComponent<VT_PlayerHealth>();
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
