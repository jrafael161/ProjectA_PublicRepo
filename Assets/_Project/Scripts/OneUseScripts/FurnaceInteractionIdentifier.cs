using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceInteractionIdentifier : MonoBehaviour
{
    public void GetInteractingFurnace()
    {
        PlayerInventory playerInventory = GetComponentInParent<UIManager>().playerInventory;
        PlayerManager playerManager = playerInventory.gameObject.GetComponent<PlayerManager>();
        PlayerInteraction playerInteraction = playerManager.gameObject.GetComponentInChildren<PlayerInteraction>();

        playerInteraction.UpdateInteractables();

        foreach (InteractionIdentifier interactable in playerInteraction.interactables)//Here i know which furnace im interacting because is inside of the player interaction volume soo ->
        {
            if (interactable.GetComponentInParent<FurnaceController>() != null)
            {
                if (interactable.GetComponentInParent<FurnaceController>().gameObject == playerManager.interactingObject)
                {
                    //PlayerManager interactingPlayer = interactable.GetComponentInParent<FurnaceController>().interactingPlayer;//I need to device a way to know which player is close and interacting with the furnace
                    interactable.GetComponentInParent<FurnaceController>().UnequipGem();
                    return;
                }
            }
        }
    }
}
