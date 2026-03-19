using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_PlayerInteraction : MonoBehaviour
{
    private List<VT_Interactable> interactables = new List<VT_Interactable>();

    private VT_Interactable closestInteractable;

    private void Start()
    {
        VT_Player player = GetComponent<VT_Player>();

        player.controls.VT_Character.Interaction.performed += context => InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        closestInteractable?.Interaction();
        interactables.Remove(closestInteractable);

        UpdateClosestInteractable();
    }

    public void UpdateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);

        closestInteractable = null;

        float closestDistance = float.MaxValue;

        foreach (VT_Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        closestInteractable?.HighlightActive(true);

    }

    public List<VT_Interactable> GetInteractables() => interactables;
}
