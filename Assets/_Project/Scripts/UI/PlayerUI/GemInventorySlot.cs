using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GemInventorySlot : MonoBehaviour
{
    public Image icon;
    public TMPro.TMP_Text itemName;
    public TMPro.TMP_Text itemDescription;
    public GemItem gemItem;
    public Button SelectButton;
    public Button unequipButton;

    public void AssignItemToSlot(GemItem newGemItem)
    {
        gemItem = newGemItem;
        icon.sprite = gemItem.itemIcon;
        itemName.text = gemItem.itemName;
        itemDescription.text = gemItem.itemDescription;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        gemItem = null;
        icon.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
        icon.enabled = false;
        gameObject.SetActive(false);
    }
}
