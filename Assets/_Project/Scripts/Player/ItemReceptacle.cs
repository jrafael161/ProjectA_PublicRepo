using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OpenningContext
{
    EquipWeapon,
    EquipObject
}

public class ItemReceptacle : MonoBehaviour
{    
    PlayerInventory playerInventory;

    public Item item;
    public WeaponItem weaponItem;
    public GemItem gemItem;
    public MaterialItem materialItem;
    public ConsumableItem consumableItem;
    public TrinketItem trinketItem;
    public KeyItem keyItem;
    public List<Item> variousItems;

    public int socketIndex;

    public OpenningContext openningContext;

    private void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
    }

    public void SetGemReceptacleFromEquipment(HandEquipmentSlotUI handEquipmentSlotUI)
    {

        if (handEquipmentSlotUI.rightHandSlot01)
        {
            if (playerInventory.weaponsInRightHandSlot[0] != null)
            {
                weaponItem = playerInventory.weaponsInRightHandSlot[0];
            }
        }
        else if (handEquipmentSlotUI.rightHandSlot02)
        {
            if (playerInventory.weaponsInRightHandSlot[1] != null)
            {
                weaponItem = playerInventory.weaponsInRightHandSlot[1];
            }
        }
        else if (handEquipmentSlotUI.leftHandSlot01)
        {
            if (playerInventory.weaponsInLeftHandSlot[0] != null)
            {
                weaponItem = playerInventory.weaponsInLeftHandSlot[0];
            }
        }
        else if (handEquipmentSlotUI.leftHandSlot02)
        {
            if (playerInventory.weaponsInLeftHandSlot[1] != null)
            {
                weaponItem = playerInventory.weaponsInLeftHandSlot[1];
            }
        }
        else
        {
            return;
        }
    }

    public void SetOpenningContext(int index)
    {
        switch (index)
        {
            case 0:
                openningContext = OpenningContext.EquipWeapon;
                break;
            case 1:
                openningContext = OpenningContext.EquipObject;
                break;
            default:
                break;
        }
    }

    public void SetGemReceptacleFromWeaponInventory(WeaponInventorySlot weaponInventorySlot)
    {
        weaponItem = weaponInventorySlot.weaponItem;
    }

    public void SetGemReceptacleFromInventory(ItemInventorySlot itemInventorySlot)
    {
        weaponItem = itemInventorySlot.item as WeaponItem;
    }

    public void SetGemToEquip(GemInventorySlot gemInventorySlot)
    {
        gemItem = gemInventorySlot.gemItem;
        EquipGem();
    }

    public void SetGemSlot(GemItem gem)
    {
        gemItem = gem;
    }

    public void ClearGemReceptacle()
    {
        weaponItem = null;
    }

    public void ClearGemToEquip()
    {
        gemItem = null;
    }

    public void ClearMaterialItem()
    {
        materialItem = null;
    }
    public void SetSocketToUse(int socketUsed)
    {
        socketIndex = socketUsed;
    }

    public void EquipGem()
    {
        switch (openningContext)
        {
            case OpenningContext.EquipWeapon:
                if (weaponItem.gemSockets[socketIndex] != null)
                {
                    weaponItem.gemSockets[socketIndex].isEquiped = false;
                }
                weaponItem.gemSockets[socketIndex] = gemItem;
                gemItem.isEquiped = true;

                weaponItem.gemSocketItemID[socketIndex] = gemItem.uniqueItemID;
                weaponItem.modelPrefab.GetComponentInChildren<GemsInObjectsManager>().CheckIfItemIsCurrentlyEquiped(weaponItem, playerInventory);
                break;

            case OpenningContext.EquipObject:
                PlayerInteraction playerInteraction = gameObject.GetComponentInChildren<PlayerInteraction>();
                FurnaceController furnaceController;

                playerInteraction.UpdateInteractables();

                foreach (InteractionIdentifier interactables in playerInteraction.interactables)
                {
                    if (interactables.GetComponent<InteractionIdentifier>() != null)
                    {
                        furnaceController = interactables.GetComponentInParent<FurnaceController>();
                        if (furnaceController != null)
                        {
                            if (furnaceController.gameObject == GetComponentInParent<PlayerManager>().interactingObject)
                            {
                                furnaceController.SetChangeGem(GetComponentInParent<PlayerManager>());
                                break;
                            }
                        }
                    }
                }   
                break;
            default:
                break;
        }

    }
}
