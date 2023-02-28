using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    PlayerInventory playerInventory;
    WeaponSlotManager weaponSlotManager;
    UIManager uIManager;

    public Image icon;
    public TMPro.TMP_Text itemName;
    public TMPro.TMP_Text itemDescription;
    public WeaponItem weaponItem;
    public Button SelectButton;
    public Button unequipButton;

    private void Start()
    {
        uIManager = GetComponentInParent<UIManager>();
        playerInventory = uIManager.playerInventory;
        weaponSlotManager = playerInventory.gameObject.GetComponentInChildren<WeaponSlotManager>();
    }

    public void AssignItemToSlot(WeaponItem newWeaponItem)
    {
        weaponItem = newWeaponItem;
        icon.sprite = weaponItem.itemIcon;
        itemName.text = newWeaponItem.itemName;
        itemDescription.text = newWeaponItem.itemDescription;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        weaponItem = null;
        icon.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
        icon.enabled = false;
        gameObject.SetActive(false);
    }

    public void EquipThisItem()//TODO: update the anim override controller
    {
        bool rightHand = false;
        bool leftHand = false;
        int equipingWeaponIndex;

        if (uIManager.rightHandSlot01Selected)
        {
            if (playerInventory.weaponsInRightHandSlot[0] != null)
                playerInventory.weaponsInRightHandSlot[0].isEquiped = false;
            
            playerInventory.weaponsInRightHandSlot[0] = weaponItem;
            weaponItem.isEquiped = true;

            rightHand = true;
            equipingWeaponIndex = 0;
        }
        else if (uIManager.rightHandSlot02Selected)
        {
            if (playerInventory.weaponsInRightHandSlot[1] != null)
                playerInventory.weaponsInRightHandSlot[1].isEquiped = false;
            
            playerInventory.weaponsInRightHandSlot[1] = weaponItem;
            weaponItem.isEquiped = true;

            rightHand = true;
            equipingWeaponIndex = 1;
        }
        else if (uIManager.leftHandSlot01Selected)
        {
            if (playerInventory.weaponsInLeftHandSlot[0] != null)
                playerInventory.weaponsInLeftHandSlot[0].isEquiped = false;
            
            playerInventory.weaponsInLeftHandSlot[0] = weaponItem;
            weaponItem.isEquiped = true;

            leftHand = true;
            equipingWeaponIndex = 0;
        }
        else if (uIManager.leftHandSlot02Selected)
        {
            if (playerInventory.weaponsInLeftHandSlot[1] != null)
                playerInventory.weaponsInLeftHandSlot[1].isEquiped = false;
            
            playerInventory.weaponsInLeftHandSlot[1] = weaponItem;
            weaponItem.isEquiped = true;

            leftHand = true;
            equipingWeaponIndex = 1;
        }
        else
        {
            return;
        }

        if (playerInventory.currentRightWeaponIndex == equipingWeaponIndex && rightHand)//avoids trying to equip the weapon when the player is barehand
        {
            playerInventory.rightHandWeapon = playerInventory.weaponsInRightHandSlot[playerInventory.currentRightWeaponIndex];

            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightHandWeapon, false);
        }

        if (playerInventory.currentLeftWeaponIndex  == equipingWeaponIndex && leftHand)//avoids trying to equip the weapon when the player is barehand
        {
            playerInventory.leftHandWeapon = playerInventory.weaponsInLeftHandSlot[playerInventory.currentLeftWeaponIndex];

            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftHandWeapon, true);
        }

        uIManager.equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);        
        uIManager.ResetAllSelectedSlots();
    }

    public void DisplayThisItem()
    {
        PlayerManager playerManager = playerInventory.gameObject.GetComponent<PlayerManager>();
        if (playerManager.interactingObject != null)
        {
            if (playerManager.interactingObject.GetComponent<WeaponDisplayerManager>() != null)
            {
                WeaponDisplayerManager weaponDisplayerManager = playerManager.interactingObject.GetComponent<WeaponDisplayerManager>();
                weaponDisplayerManager.DisplayObject(weaponItem);
            }
        }
    }
}
