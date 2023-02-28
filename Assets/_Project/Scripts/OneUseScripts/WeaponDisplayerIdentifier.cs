using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDisplayerIdentifier : MonoBehaviour
{
    public void GetInteractingWeaponDisplayer()
    {
        PlayerInventory playerInventory = GetComponentInParent<UIManager>().playerInventory;
        PlayerManager playerManager = playerInventory.gameObject.GetComponent<PlayerManager>();
        PlayerInteraction playerInteraction = playerManager.gameObject.GetComponentInChildren<PlayerInteraction>();

        playerInteraction.UpdateInteractables();

        foreach (InteractionIdentifier interactable in playerInteraction.interactables)//Here i know which furnace im interacting because is inside of the player interaction volume soo ->
        {
            if (interactable.GetComponent<WeaponDisplayerManager>() != null)
            {
                if (interactable.GetComponent<WeaponDisplayerManager>().gameObject == playerManager.interactingObject)
                {
                    interactable.GetComponent<WeaponDisplayerManager>().RetrieveDisplayedWeapon();
                    return;
                }
            }
        }
    }
}
