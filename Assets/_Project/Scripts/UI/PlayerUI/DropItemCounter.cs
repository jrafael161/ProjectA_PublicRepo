using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropItemCounter : MonoBehaviour
{
    public TMPro.TMP_Text dropItemsCuantity_txt;
    public Image itemIcon;
    public Button plusButton;
    public Button minusButton;

    PlayerInventory playerInventory;

    public int cuantity;

    private void Awake()
    {
        playerInventory = GetComponentInParent<UIManager>().playerInventory;
    }

    private void OnEnable()
    {
        if (playerInventory == null)
        {
            playerInventory = GetComponentInParent<UIManager>().playerInventory;
        }

        if (playerInventory.dropedItem != null)
        {
            itemIcon.sprite = playerInventory.dropedItem.itemIcon;
        }
        cuantity = 1;
        dropItemsCuantity_txt.text = "x1";
        minusButton.interactable = false;
        plusButton.interactable = true;
    }

    public void IncreaseCuantity()
    {
        int maxCuantity = playerInventory.GetDropedItemCuantity();
        if (cuantity < maxCuantity)
        {
            cuantity++;
            dropItemsCuantity_txt.text = "x" + cuantity.ToString();
            if (cuantity == maxCuantity)
            {
                plusButton.interactable = false;
            }
            minusButton.interactable = true;
        }
    }

    public void DecreaseCuantity()
    {
        if (cuantity >= 2)
        {
            cuantity--;
            dropItemsCuantity_txt.text = "x" + cuantity.ToString();
            if (cuantity == 1)
            {
                minusButton.interactable = false;
            }
            plusButton.interactable = true;
        }
    }
}
