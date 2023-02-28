using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoInventorySlot : MonoBehaviour
{
    UIManager uIManager;

    public Image icon;
    public TMPro.TMP_Text itemName;
    public TMPro.TMP_Text itemDescription;
    public TMPro.TMP_Text stackCuantity;
    public AmmoItem ammoItem;
    public Button SelectButton;
    public Button unequipButton;

    private void Start()
    {
        uIManager = GetComponentInParent<UIManager>();
    }


    public void AssignItemToSlot(AmmoItem newAmmoItem)
    {
        ammoItem = newAmmoItem;
        icon.sprite = ammoItem.itemIcon;
        itemName.text = ammoItem.itemName;
        itemDescription.text = ammoItem.itemDescription;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        ammoItem = null;
        icon.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
        icon.enabled = false;
        gameObject.SetActive(false);
    }

    public void EquipThisAmmo(AmmoInventorySlot ammoInventorySlot)//TODO: update the anim override controller
    {
        if (uIManager.ammoSlotSelected)
        {
            if (uIManager.playerInventory.ammoSlot != null)
                uIManager.playerInventory.ammoSlot = null;

            uIManager.playerInventory.ammoSlot = ammoItem;
            uIManager.ammoSlotCuantity_txt.text = ammoInventorySlot.stackCuantity.text;
            uIManager.ammoSlotCuantity_txt.gameObject.SetActive(true);
        }
        else
        {
            uIManager.ammoSlotCuantity_txt.text = "x0";
            uIManager.ammoSlotCuantity_txt.gameObject.SetActive(false);
            return;
        }

        uIManager.equipmentWindowUI.LoadWeaponsOnEquipmentScreen(uIManager.playerInventory);
        uIManager.ResetAllSelectedSlots();
    }
}
