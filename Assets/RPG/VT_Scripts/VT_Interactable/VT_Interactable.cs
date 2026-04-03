using UnityEngine;

public class VT_Interactable : MonoBehaviour
{
    protected VT_Player_WeaponController weaponController;

    private MeshRenderer mesh;
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;

    private void Start()
    {
        if (mesh == null)
        {
            mesh = GetComponentInChildren<MeshRenderer>();
        }

        defaultMaterial = mesh.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interaction()
    {
        Debug.Log("Va cham voi: " + gameObject.name);
    }

    public void HighlightActive(bool active)
    {
        if (active)
        {
            mesh.material = highlightMaterial;
        }
        else
        {
            mesh.material = defaultMaterial;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            HighlightActive(true);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponController == null)
        {
            weaponController = other.GetComponent<VT_Player_WeaponController>();
        }

        VT_Player_Interaction playerInteraction = other.GetComponent<VT_Player_Interaction>();

        if (playerInteraction == null)
        {
            return;
        }

        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        VT_Player_Interaction playerInteraction = other.GetComponent<VT_Player_Interaction>();

        if (playerInteraction == null)
        {
            return;
        }

        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}
