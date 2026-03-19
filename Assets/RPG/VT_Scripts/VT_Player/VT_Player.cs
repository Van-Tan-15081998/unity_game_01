using UnityEngine;

public class VT_Player : MonoBehaviour
{
    public PlayerControls controls { get; private set; } // Read-Only
    public VT_PlayerAim aim { get; private set; } // Read-Only

    public VT_PlayerMovement movement { get; private set; } // Read-Only

    public VT_PlayerWeaponController weapon { get; private set; } // Read-Only

    public VT_PlayerWeaponVisuals weaponVisuals { get; private set; } // Read-Only

    public VT_PlayerInteraction interaction { get; private set; } // Read-Only

    private void Awake()
    {
        controls = new PlayerControls();
        aim = GetComponent<VT_PlayerAim>();
        movement = GetComponent<VT_PlayerMovement>();   
        weapon = GetComponent<VT_PlayerWeaponController>();
        weaponVisuals = GetComponent<VT_PlayerWeaponVisuals>();
        interaction = GetComponent<VT_PlayerInteraction>();
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
