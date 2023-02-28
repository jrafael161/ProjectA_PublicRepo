using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting.APIUpdating;

//[MovedFrom(false, null, null, "WeaponGemSocketManager")]
public class WeaponGemSocketManager : MonoBehaviour
{
    PlayerInventory playerInventory;

    WeaponItem weaponItem;

    public Button GemSocket01;
    public Button GemSocket02;
    public Button GemSocket03;

    private void OnEnable()
    {
        playerInventory = GetComponentInParent<UIManager>().playerInventory;

        HandEquipmentSlotUI handEquipmentSlotUI = GetComponentInParent<HandEquipmentSlotUI>();
        WeaponInventorySlot weaponInventorySlot = transform.parent.GetComponentInParent<WeaponInventorySlot>();
        ItemInventorySlot itemInventorySlot = transform.parent.GetComponentInParent<ItemInventorySlot>();

        if ((handEquipmentSlotUI == null && weaponInventorySlot == null) && itemInventorySlot ==null)
            return;

        if (handEquipmentSlotUI != null)//if the gem is bein equiped from the equimpent window
        {
            if (handEquipmentSlotUI.rightHandSlot01)
            {
                weaponItem = playerInventory.weaponsInRightHandSlot[0];
            }
            else if (handEquipmentSlotUI.rightHandSlot02)
            {
                weaponItem = playerInventory.weaponsInRightHandSlot[1];
            }
            else if (handEquipmentSlotUI.leftHandSlot01)
            {
                weaponItem = playerInventory.weaponsInLeftHandSlot[0];
            }
            else
            {
                weaponItem = playerInventory.weaponsInLeftHandSlot[1];
            }
        }

        if (weaponInventorySlot != null)//if the gem is bein equiped from the weapon inventory window
            weaponItem = weaponInventorySlot.weaponItem;

        if (itemInventorySlot != null)//if the gem is bein equiped from the inventory window
        {
            weaponItem = itemInventorySlot.item as WeaponItem;
        }

        if (weaponItem == null)//If the first item inventory slot had its gem slots enabled it was leaving the slots enabled for its subsequential copies
        {
            GemSocket01.gameObject.SetActive(false);
            GemSocket02.gameObject.SetActive(false);
            GemSocket03.gameObject.SetActive(false);
            return;
        }

        if (weaponItem.gemSockets.Length > 0)
        {
            int GemSockets = weaponItem.gemSockets.Length;
            switch (GemSockets)
            {
                case 1:
                    GemSocket01.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[0] != null)
                    {
                        GemSocket01.image.sprite = weaponItem.gemSockets[0].itemIcon;
                    }
                    else
                    {
                        GemSocket01.image.sprite = null;
                    }
                    GemSocket02.gameObject.SetActive(false);
                    GemSocket03.gameObject.SetActive(false);
                    break;
                case 2:

                    GemSocket01.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[0] != null)
                    {
                        GemSocket01.image.sprite = weaponItem.gemSockets[0].itemIcon;
                    }
                    else
                    {
                        GemSocket01.image.sprite = null;
                    }

                    GemSocket02.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[1] != null)
                    {
                        GemSocket02.image.sprite = weaponItem.gemSockets[1].itemIcon;
                    }
                    else
                    {
                        GemSocket02.image.sprite = null;
                    }
                    GemSocket03.gameObject.SetActive(false);
                    break;
                case 3:

                    GemSocket01.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[0] != null)
                    {
                        GemSocket01.image.sprite = weaponItem.gemSockets[0].itemIcon;
                    }
                    else
                    {
                        GemSocket01.image.sprite = null;
                    }

                    GemSocket02.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[1] != null)
                    {
                        GemSocket02.image.sprite = weaponItem.gemSockets[1].itemIcon;
                    }
                    else
                    {
                        GemSocket02.image.sprite = null;
                    }

                    GemSocket03.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[2] != null)
                    {
                        GemSocket03.image.sprite = weaponItem.gemSockets[2].itemIcon;
                    }
                    else
                    {
                        GemSocket03.image.sprite = null;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            GemSocket01.gameObject.SetActive(false);
            GemSocket02.gameObject.SetActive(false);
            GemSocket03.gameObject.SetActive(false);
        }
    }

    public void UpdateGemSockets()
    {
        HandEquipmentSlotUI handEquipmentSlotUI = GetComponentInParent<HandEquipmentSlotUI>();
        WeaponInventorySlot weaponInventorySlot = transform.parent.GetComponentInParent<WeaponInventorySlot>();
        ItemInventorySlot itemInventorySlot = transform.parent.GetComponentInParent<ItemInventorySlot>();

        if ((handEquipmentSlotUI == null && weaponInventorySlot == null) && itemInventorySlot == null)
            return;

        if (handEquipmentSlotUI != null)//if the gem is bein equiped from the equimpent window
        {
            if (handEquipmentSlotUI.rightHandSlot01)
            {
                weaponItem = playerInventory.weaponsInRightHandSlot[0];
            }
            else if (handEquipmentSlotUI.rightHandSlot02)
            {
                weaponItem = playerInventory.weaponsInRightHandSlot[1];
            }
            else if (handEquipmentSlotUI.leftHandSlot01)
            {
                weaponItem = playerInventory.weaponsInLeftHandSlot[0];
            }
            else
            {
                weaponItem = playerInventory.weaponsInLeftHandSlot[1];
            }
        }

        if (weaponInventorySlot != null)//if the gem is bein equiped from the inventory window
            weaponItem = weaponInventorySlot.weaponItem;

        if (itemInventorySlot != null)
        {
            weaponItem = itemInventorySlot.item as WeaponItem;
        }

        if (weaponItem == null)//If the first item inventory slot had its gem slots enabled it was leaving the slots enabled for its subsequential copies
        {
            GemSocket01.gameObject.SetActive(false);
            GemSocket02.gameObject.SetActive(false);
            GemSocket03.gameObject.SetActive(false);
            return;
        }

        if (weaponItem.gemSockets.Length > 0)
        {
            int GemSockets = weaponItem.gemSockets.Length;
            switch (GemSockets)
            {
                case 1:
                    GemSocket01.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[0] != null)
                    {
                        GemSocket01.image.sprite = weaponItem.gemSockets[0].itemIcon;
                    }
                    else
                    {
                        GemSocket01.image.sprite = null;
                    }
                    GemSocket02.gameObject.SetActive(false);
                    GemSocket03.gameObject.SetActive(false);
                    break;
                case 2:

                    GemSocket01.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[0] != null)
                    {
                        GemSocket01.image.sprite = weaponItem.gemSockets[0].itemIcon;
                    }
                    else
                    {
                        GemSocket01.image.sprite = null;
                    }

                    GemSocket02.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[1] != null)
                    {
                        GemSocket02.image.sprite = weaponItem.gemSockets[1].itemIcon;
                    }
                    else
                    {
                        GemSocket02.image.sprite = null;
                    }
                    GemSocket03.gameObject.SetActive(false);
                    break;
                case 3:

                    GemSocket01.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[0] != null)
                    {
                        GemSocket01.image.sprite = weaponItem.gemSockets[0].itemIcon;
                    }
                    else
                    {
                        GemSocket01.image.sprite = null;
                    }

                    GemSocket02.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[1] != null)
                    {
                        GemSocket02.image.sprite = weaponItem.gemSockets[1].itemIcon;
                    }
                    else
                    {
                        GemSocket02.image.sprite = null;
                    }

                    GemSocket03.gameObject.SetActive(true);
                    if (weaponItem.gemSockets[2] != null)
                    {
                        GemSocket03.image.sprite = weaponItem.gemSockets[2].itemIcon;
                    }
                    else
                    {
                        GemSocket03.image.sprite = null;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            GemSocket01.gameObject.SetActive(false);
            GemSocket02.gameObject.SetActive(false);
            GemSocket03.gameObject.SetActive(false);
        }
    }
}
