using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public EquipmentWindowUI equipmentWindowUI;

    [Header("UI Windows")]
    public GameObject playerUI;
    public GameObject inGameMenu;
    public GameObject equipmentWindow;
    public GameObject weaponInventoryWindow;
    public GameObject gemInventoryWindow;
    public GameObject ammoInventoryWindow;
    public GameObject itemInventoryWindow;
    public GameObject inventoryTopWindow;
    public GameObject gameOptionsWindow;
    public GameObject objectInventoryWindow;
    public GameObject ItemDropWindow;
    public GameObject DeadScreen;

    [Header("Equipment Window Slot Selected")]
    public bool rightHandSlot01Selected;
    public bool rightHandSlot02Selected;
    public bool leftHandSlot01Selected;
    public bool leftHandSlot02Selected;
    public bool ammoSlotSelected;
    public TMPro.TMP_Text ammoSlotCuantity_txt;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPrefab;
    public Transform weaponInventorySlotsParent;
    List<WeaponInventorySlot> weaponInventorySlots;

    [Header("Item Inventory")]
    public GameObject itemInventorySlotPrefab;
    public Transform itemInventorySlotsParent;
    List<ItemInventorySlot> itemInventorySlots;

    [Header("Gem Inventory")]
    public GameObject gemInventorySlotPrefab;
    public Transform gemInventorySlotsParent;
    List<GemInventorySlot> gemInventorySlots;

    [Header("Ammo Inventory")]
    public GameObject ammoInventorySlotPrefab;
    public Transform ammoInventorySlotsParent;
    List<AmmoInventorySlot> ammoInventorySlots;

    [Header("Interaction prompts")]
    public GameObject itemAdquisitionMessagePrompt;
    public GameObject interactionMessagePrompt;
    public GameObject interactionMessageTogglePrompt;

    List<Item> alreadyStackedtackedItems;
    List<Item> alreadyStackedtackedAmmo;

    public List<GameObject> OpenWindows;
    private void Start()
    {
        weaponInventorySlots = new List<WeaponInventorySlot>();
        ammoInventorySlots = new List<AmmoInventorySlot>();
        itemInventorySlots = new List<ItemInventorySlot>();
        gemInventorySlots = new List<GemInventorySlot>();
        alreadyStackedtackedItems = new List<Item>();
        alreadyStackedtackedAmmo = new List<Item>();
    }

    public void UpdateUI(int itemType)//inventory index: from which inventory is going to pull items to show in th UI
    {
        itemInventorySlots.Clear();
        alreadyStackedtackedItems.Clear();
        playerInventory.itemStacks.Clear();

        foreach (ItemInventorySlot itemSlot in itemInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>(true))
        {
            itemInventorySlots.Add(itemSlot);
        }

        if (itemInventorySlots.Count < playerInventory.itemsInventory.Count)
        {
            while (itemInventorySlots.Count < playerInventory.itemsInventory.Count)
            {
                GameObject aux = Instantiate(itemInventorySlotPrefab, itemInventorySlotsParent);
                itemInventorySlots.Add(aux.GetComponentInChildren<ItemInventorySlot>());
            }
        }
        else
        {
            while (itemInventorySlots.Count > playerInventory.itemsInventory.Count && itemInventorySlots.Count > 1)
            {
                GameObject aux = itemInventorySlots[itemInventorySlots.Count - 1].gameObject;
                itemInventorySlots.RemoveAt(itemInventorySlots.Count - 1);
                Destroy(aux);
            }
            if (playerInventory.itemsInventory.Count == 0)//There are no items in the inventory
            {
                GameObject aux = itemInventorySlots[itemInventorySlots.Count - 1].gameObject;
                aux.SetActive(false);
            }
        }

        for (int i = 0; i < playerInventory.itemsInventory.Count; i++)
        {
            if (playerInventory.itemsInventory[i].itemType == (ItemType)itemType)
            {
                if (playerInventory.itemsInventory[i].isStackable == true && !alreadyStackedtackedItems.Find(x=>x.itemName == playerInventory.itemsInventory[i].itemName))
                {
                    itemInventorySlots[i].AssignItemToSlot(playerInventory.itemsInventory[i]);
                    itemInventorySlots[i].stackCuantity.gameObject.SetActive(true);
                    List<Item>stack = playerInventory.itemsInventory.FindAll(x => x.itemName == playerInventory.itemsInventory[i].itemName);//Bit of a mouthfull isnt it?
                    if (stack.Count > 1)
                    {
                        playerInventory.itemStacks.Add(stack);
                    }                    
                    itemInventorySlots[i].stackCuantity.text = "x" + stack.Count.ToString();
                    alreadyStackedtackedItems.Add(playerInventory.itemsInventory[i]);
                    if (playerInventory.itemsInventory[i].itemType == ItemType.Gem || playerInventory.itemsInventory[i].itemType == ItemType.Weapon)
                    {
                        itemInventorySlots[i].UnequipButton.interactable = true;
                    }
                    else
                    {
                        itemInventorySlots[i].UnequipButton.interactable = false;
                    }
                }
                else if (playerInventory.itemsInventory[i].isStackable == false)
                {
                    itemInventorySlots[i].AssignItemToSlot(playerInventory.itemsInventory[i]);
                    itemInventorySlots[i].stackCuantity.text = "";
                    itemInventorySlots[i].stackCuantity.gameObject.SetActive(false);
                }
                else
                {
                    itemInventorySlots[i].gameObject.SetActive(false);
                }
            }
            else
            {
                itemInventorySlots[i].gameObject.SetActive(false);
            }
        }

        foreach (var itemInventorySlot in itemInventorySlots)
        {
            if (itemInventorySlot.GetComponentInChildren<WeaponGemSocketManager>().GemSocket01.gameObject.activeSelf)
                itemInventorySlot.GetComponentInChildren<WeaponGemSocketManager>().UpdateGemSockets();
        }
    }

    public void ShowWeaponItemSlots()
    {
        weaponInventorySlots.Clear();
        foreach (WeaponInventorySlot itemSlot in weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>(true))
        {
            weaponInventorySlots.Add(itemSlot);
        }

        if (weaponInventorySlots.Count < playerInventory.itemsInventory.Count)
        {
            while (weaponInventorySlots.Count < playerInventory.itemsInventory.Count)
            {
                GameObject aux = Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent);
                weaponInventorySlots.Add(aux.GetComponentInChildren<WeaponInventorySlot>());
            }
        }
        else
        {
            while (weaponInventorySlots.Count > playerInventory.itemsInventory.Count && weaponInventorySlots.Count > 1)
            {
                GameObject aux = weaponInventorySlots[weaponInventorySlots.Count - 1].gameObject;
                weaponInventorySlots.RemoveAt(weaponInventorySlots.Count - 1);
                Destroy(aux);
            }
        }

        for (int i = 0; i < playerInventory.itemsInventory.Count; i++)
        {
            if (playerInventory.itemsInventory[i].itemType == ItemType.Weapon)
            {
                weaponInventorySlots[i].AssignItemToSlot(playerInventory.itemsInventory[i] as WeaponItem);
                WeaponItem aux = playerInventory.itemsInventory[i] as WeaponItem;
                if (aux.isEquiped)
                {
                    Button WeaponSlot = weaponInventorySlots[i].GetComponentInChildren<Button>();
                    WeaponSlot.interactable = false;
                    weaponInventorySlots[i].unequipButton.interactable = true;
                }
                else
                {
                    Button WeaponSlot = weaponInventorySlots[i].GetComponentInChildren<Button>();
                    WeaponSlot.interactable = true;
                    weaponInventorySlots[i].unequipButton.interactable = false;
                }
            }
            else
            {
                weaponInventorySlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowGemItemSlots()
    {
        gemInventorySlots.Clear();
        foreach (GemInventorySlot itemSlot in gemInventorySlotsParent.GetComponentsInChildren<GemInventorySlot>(true))
        {
            gemInventorySlots.Add(itemSlot);
        }

        if (gemInventorySlots.Count < playerInventory.itemsInventory.Count)
        {
            while (gemInventorySlots.Count < playerInventory.itemsInventory.Count)
            {
                GameObject aux = Instantiate(gemInventorySlotPrefab, gemInventorySlotsParent);
                gemInventorySlots.Add(aux.GetComponentInChildren<GemInventorySlot>());
            }
        }
        else
        {
            while (gemInventorySlots.Count > playerInventory.itemsInventory.Count && gemInventorySlots.Count > 1)
            {
                GameObject aux = gemInventorySlots[gemInventorySlots.Count - 1].gameObject;
                gemInventorySlots.RemoveAt(gemInventorySlots.Count - 1);
                Destroy(aux);
            }
        }

        for (int i = 0; i < playerInventory.itemsInventory.Count; i++)
        {
            if (playerInventory.itemsInventory[i].itemType == ItemType.Gem)
            {
                gemInventorySlots[i].AssignItemToSlot(playerInventory.itemsInventory[i] as GemItem);
                GemItem aux = playerInventory.itemsInventory[i] as GemItem;
                if (aux.isEquiped)
                {
                    Button GemSlot = gemInventorySlots[i].GetComponentInChildren<Button>();
                    GemSlot.interactable = false;
                    gemInventorySlots[i].unequipButton.interactable = true;
                }
                else
                {
                    Button GemSlot = gemInventorySlots[i].GetComponentInChildren<Button>();
                    GemSlot.interactable = true;
                    gemInventorySlots[i].unequipButton.interactable = false;
                }
            }
            else
            {
                gemInventorySlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowAmmoItemSlots()
    {
        ammoInventorySlots.Clear();
        alreadyStackedtackedAmmo.Clear();
        playerInventory.ammoStacks.Clear();

        foreach (AmmoInventorySlot itemSlot in ammoInventorySlotsParent.GetComponentsInChildren<AmmoInventorySlot>(true))
        {
            ammoInventorySlots.Add(itemSlot);
        }

        List<Item> allAmmoInInventory = playerInventory.itemsInventory.Where(x => x.itemType == ItemType.Ammo).ToList();

        foreach (Item ammo in allAmmoInInventory)
        {
            if (!alreadyStackedtackedAmmo.Find(x => x.itemName == ammo.itemName))
            {
                List<Item> stack = playerInventory.itemsInventory.FindAll(x => x.itemName == ammo.itemName);
                if (stack.Count >= 1)
                {
                    playerInventory.ammoStacks.Add(stack);
                }
                alreadyStackedtackedAmmo.Add(ammo);
            }
        }

        if (ammoInventorySlots.Count < alreadyStackedtackedAmmo.Count)
        {
            while (ammoInventorySlots.Count < alreadyStackedtackedAmmo.Count)
            {
                GameObject aux = Instantiate(ammoInventorySlotPrefab, ammoInventorySlotsParent);
                ammoInventorySlots.Add(aux.GetComponentInChildren<AmmoInventorySlot>());
            }
        }
        else if (alreadyStackedtackedAmmo.Count > ammoInventorySlots.Count && ammoInventorySlots.Count > 1)
        { 
            while (alreadyStackedtackedAmmo.Count > ammoInventorySlots.Count && ammoInventorySlots.Count > 1)
            {
                GameObject aux = ammoInventorySlots[ammoInventorySlots.Count - 1].gameObject;
                ammoInventorySlots.RemoveAt(ammoInventorySlots.Count - 1);
                Destroy(aux);
            }
        }
        else
        {
            ammoInventorySlots[0].gameObject.SetActive(false);
        }

        for (int i = 0; i < alreadyStackedtackedAmmo.Count; i++)
        {
            ammoInventorySlots[i].AssignItemToSlot(alreadyStackedtackedAmmo[i] as AmmoItem);
            ammoInventorySlots[i].stackCuantity.text = "x" + playerInventory.ammoStacks[i].Count().ToString();
        }
    }

    public void UnequipItem(ItemInventorySlot itemInventorySlot)//Action done from a button in the Items inventory menu UI
    {
        switch (itemInventorySlot.item.itemType)
        {
            case ItemType.Consumable:
                //TODO: create a function to handle this
                break;
            case ItemType.KeyItem:
                //Cannot be equiped therefore unequiped or could they?
                break;
            case ItemType.Gem:
                playerInventory.UnequipGem(itemInventorySlot.item as GemItem);
                break;
            case ItemType.Weapon:
                playerInventory.UnequipWeapon(itemInventorySlot.item as WeaponItem);
                break;
            case ItemType.Ammo:
                playerInventory.UnequipAmmo(itemInventorySlot.item as AmmoItem);
                break;
            case ItemType.Material:
                //Cannot be equiped therefore unequiped
                break;
            case ItemType.Trinket:
                //TODO: create a function to handle this
                break;
            default:
                break;
        }
    }

    public void UnequipWeaponItem(WeaponInventorySlot weaponInventorySlot)
    {
        playerInventory.UnequipWeapon(weaponInventorySlot.weaponItem);
        weaponInventorySlot.SelectButton.interactable = true;
    }

    public void UnequipGemItem(GemInventorySlot gemInventorySlot)
    {
        playerInventory.UnequipGem(gemInventorySlot.gemItem);
        gemInventorySlot.SelectButton.interactable = true;
    }

    public void UnequipAmmoItem(AmmoInventorySlot ammoInventorySlot)//Action done from Ammo inventory UI
    {
        playerInventory.UnequipAmmo(ammoInventorySlot.ammoItem);
        ammoInventorySlot.SelectButton.interactable = true;
        ammoSlotCuantity_txt.gameObject.SetActive(false);
    }

    public void OpenInGameMenu()
    {
        inGameMenu.SetActive(true);
        OpenWindows.Add(inGameMenu);
    }

    public void CloseAllUIWindows()
    {
        ResetAllSelectedSlots();

        inGameMenu.SetActive(false);
        weaponInventoryWindow.SetActive(false);
        equipmentWindow.SetActive(false);
        itemInventoryWindow.SetActive(false);
        gemInventoryWindow.SetActive(false);
        inventoryTopWindow.SetActive(false);
        gameOptionsWindow.SetActive(false);
        objectInventoryWindow.SetActive(false);
        ItemDropWindow.SetActive(false);
    }

    public void InitilizeWeaponInventorySlots()
    {
        if (equipmentWindowUI == null)
        {
            equipmentWindowUI = equipmentWindow.GetComponentInChildren<EquipmentWindowUI>();
        }
        equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
    }

    public void ResetAllSelectedSlots()
    {
        rightHandSlot01Selected = false;
        rightHandSlot02Selected = false;
        leftHandSlot01Selected = false;
        leftHandSlot02Selected = false;
        ammoSlotSelected = false;
    }

    public void OpenPreviousWindow()
    {
        OpenWindows[OpenWindows.Count-1].SetActive(true);
    }

    public void AddOpenWindow(GameObject window)
    {
        OpenWindows.Add(window);
    }

    public void RemoveOpenWindow()
    {
        OpenWindows[OpenWindows.Count-1].SetActive(false);
        OpenWindows.RemoveAt(OpenWindows.Count - 1);
    }

    public void RemoveOpenWindow(GameObject window)
    {
        window.SetActive(false);
        OpenWindows.Remove(window);
    }
}
