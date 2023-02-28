using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemInventorySlot : MonoBehaviour
{
    public Image icon;
    public TMPro.TMP_Text itemName;
    public TMPro.TMP_Text itemDescription;
    public TMPro.TMP_Text stackCuantity;
    public Item item;
    public Button SelectButton;
    public Button UnequipButton;

    public void AssignItemToSlot(Item newItem)
    {
        item = newItem;
        icon.sprite = item.itemIcon;
        itemName.text = newItem.itemName;
        itemDescription.text = newItem.itemDescription;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        item = null;
        icon.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
        icon.enabled = false;
        gameObject.SetActive(false);
    }

    public void EquipItem()
    {
        //TODO equip consumable items or gems
    }

    public void UseItem()
    {
        //TODO use consumable items or gems
    }
}
