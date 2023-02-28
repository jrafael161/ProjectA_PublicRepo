using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWindowUI : MonoBehaviour
{
    public bool rightHandSlot01Selected;
    public bool rightHandSlot02Selected;
    public bool leftHandSlot01Selected;
    public bool leftHandSlot02Selected;
    public bool ammoSlotSelected;

    HandEquipmentSlotUI[] handEquipmentSlotUI;
    public void LoadWeaponsOnEquipmentScreen(PlayerInventory playerInventory)
    {
        handEquipmentSlotUI = GetComponentsInChildren<HandEquipmentSlotUI>();
        for (int i = 0; i < handEquipmentSlotUI.Length; i++)
        {
            if (handEquipmentSlotUI[i].rightHandSlot01)
            {
                handEquipmentSlotUI[i].SelectWeaponItem(playerInventory.weaponsInRightHandSlot[0]);
            }
            else if (handEquipmentSlotUI[i].rightHandSlot02)
            {
                handEquipmentSlotUI[i].SelectWeaponItem(playerInventory.weaponsInRightHandSlot[1]);
            }
            else if (handEquipmentSlotUI[i].leftHandSlot01)
            {
                handEquipmentSlotUI[i].SelectWeaponItem(playerInventory.weaponsInLeftHandSlot[0]);
            }
            else if(handEquipmentSlotUI[i].leftHandSlot02)
            {
                handEquipmentSlotUI[i].SelectWeaponItem(playerInventory.weaponsInLeftHandSlot[1]);
            }
            else
            {
                int ammoAvailable = playerInventory.IsAmmoAvailable();
                if (ammoAvailable >= 1)
                {
                    UIManager uIManager = GetComponentInParent<UIManager>();
                    uIManager.ammoSlotCuantity_txt.text = ammoAvailable.ToString();
                    handEquipmentSlotUI[i].SelectAmmoItem(playerInventory.ammoSlot);
                }
                else
                {
                    UIManager uIManager = GetComponentInParent<UIManager>();
                    uIManager.ammoSlotCuantity_txt.gameObject.SetActive(false);
                    playerInventory.UnequipAmmo(handEquipmentSlotUI[i].ammo);
                }
            }
        }
    }
    public void SelectRightHandSlot01()
    {
        rightHandSlot01Selected = true;
    }
    public void SelectRightHandSlot02()
    {
        rightHandSlot02Selected = true;
    }

    public void SelectLeftHandSlot01()
    {
        leftHandSlot01Selected = true;
    }
    public void SelectLeftHandSlot02()
    {
        leftHandSlot02Selected = true;
    }

    public void SelectAmmoSlot()
    {
        ammoSlotSelected = true;
    }
}
