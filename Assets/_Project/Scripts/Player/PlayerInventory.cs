using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInventory : MonoBehaviour
{
    PlayerManager _playerManager;
    QuickSlotsUI _quickSlots;
    public WeaponSlotManager _weaponSlotManager;

    public WeaponItem rightHandWeapon;
    public WeaponItem leftHandWeapon;
    public WeaponItem backSlotWeapon;

    public WeaponItem rightHandHiddenWeapon;
    public WeaponItem leftHandHiddenWeapon;

    public WeaponItem unarmedWeapon;

    public WeaponItem[] weaponsInRightHandSlot = new WeaponItem[2];
    public WeaponItem[] weaponsInLeftHandSlot = new WeaponItem[2];
    public AmmoItem ammoSlot;

    public int currentRightWeaponIndex = -1;
    public int currentLeftWeaponIndex = -1;

    public List<Item> itemsInventory;
    public List<List<Item>> itemStacks;
    public List<List<Item>> ammoStacks;
    public List<string> _serializedItemsInventory;//Could be turned to a dictionary to manage more efficiently the ins and outs of the inventory

    public Item dropedItem;//I should guet rid of this to avoid usage confusion

    private void Start()
    {
        itemStacks = new List<List<Item>>();
        ammoStacks = new List<List<Item>>();
        _weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        _playerManager = GetComponent<PlayerManager>();

        CreateUnarmedWeaponInstances();

        _weaponSlotManager.LoadWeaponOnSlot(rightHandWeapon, false);
        _weaponSlotManager.LoadWeaponOnSlot(leftHandWeapon, true);

        _quickSlots = _playerManager.UIManager.GetComponentInChildren<QuickSlotsUI>(true);
    }

    public void CreateUnarmedWeaponInstances()
    {
        if (unarmedWeapon != null)
        {
            WeaponItem weaponItem = Instantiate(unarmedWeapon);
            rightHandWeapon = null;
            rightHandWeapon = weaponItem;
            leftHandWeapon = null;
            leftHandWeapon = weaponItem;
            unarmedWeapon = null;
            unarmedWeapon = weaponItem;
        }
    }

    public void ChangeRightWeapon()
    {
        _playerManager.gameObject.GetComponentInChildren<PlayerCombatHandler>().WeaponChangeCurrentActionCancel();

        currentRightWeaponIndex += 1;
        if (currentRightWeaponIndex > weaponsInRightHandSlot.Length - 1)
        {
            currentRightWeaponIndex = -1;
            rightHandWeapon = unarmedWeapon;
            _playerManager.isTwoHandingWeapon = false;//If the weapon index is reseted it must be a unnarmed weapon in one hand
            _playerManager.gameObject.GetComponent<InputHandler>().twoHandedFlag = false;
            _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
            return;
        }

        if (weaponsInRightHandSlot[currentRightWeaponIndex] != null)//The currentWeaponIndex is getting offseted if the player doesnt hold anything in the next slot, maybe equip an unarmed weapon so it changes to that slot
        {
            rightHandWeapon = weaponsInRightHandSlot[currentRightWeaponIndex];
            _weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlot[currentRightWeaponIndex], false);
        }
    }

    public void ChangeLeftWeapon()
    {
        _playerManager.gameObject.GetComponentInChildren<PlayerCombatHandler>().WeaponChangeCurrentActionCancel();

        currentLeftWeaponIndex += 1;
        if (currentLeftWeaponIndex > weaponsInLeftHandSlot.Length - 1)
        {
            currentLeftWeaponIndex = -1;
            leftHandWeapon = unarmedWeapon;
            _playerManager.isTwoHandingWeapon = false;//If the weapon index is reseted it must be a unnarmed weapon in one hand
            _playerManager.gameObject.GetComponent<InputHandler>().twoHandedFlag = false;
            _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
            return;
        }

        if (weaponsInLeftHandSlot[currentLeftWeaponIndex] != null)
        {
            leftHandWeapon = weaponsInLeftHandSlot[currentLeftWeaponIndex];
            _weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlot[currentLeftWeaponIndex], true);
        }
    }

    public int GetDropedItemCuantity()
    {
        for (int i = 0; i < itemStacks.Count; i++)
        {
            if (itemStacks[i].Contains(dropedItem))
            {
                return itemStacks[i].Count;
            }
        }
        return 0;
    }

    public void AddItem(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            WeaponItem weaponItem = item as WeaponItem;
            for (int i = 0; i < weaponItem.gemSockets.Length; i++)
            {
                if (weaponItem.gemSockets[i] != null)
                {
                    AddItem(weaponItem.gemSockets[i]);
                }
            }
        }

        if (item.itemType == ItemType.Bundle)
        {
            ItemsBundle itemsBundle = item as ItemsBundle;
            foreach (var itemInsideBundle in itemsBundle.itemsBundle)
            {
                Item bundleItem = Instantiate(itemInsideBundle);
                AddItem(bundleItem);
            }
            return;
        }

        if (item.itemType == ItemType.Ammo)
        {
            AmmoItem ammoItem = item as AmmoItem;
            ammoItem.isStuck = false;
        }

        if (item.uniqueItemID == "" || item.uniqueItemID == null)
        {
            item.uniqueItemID = System.Guid.NewGuid().ToString();
        }

        itemsInventory.Add(item);
        //_serializedItemsInventory.Add(SerializationController._instance.SerializeItem(item)); a bit pointless because anyway all the inventory gets serialized when the player saves
    }

    public void AddWeaponFromSaveFile(Item item)//This evades duplication of gems when using the AddItem function when loading the save
    {
        itemsInventory.Add(item);
    }

    public void ReconstructWeapon(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            WeaponItem weaponItem = item as WeaponItem;
            for (int i = 0; i < weaponItem.gemSockets.Length; i++)
            {
                if (weaponItem.gemSocketItemID[i] != null)
                {
                    Item supposedlyGemToEquip = itemsInventory.Find(x => x.uniqueItemID == weaponItem.gemSocketItemID[i]);
                    if (supposedlyGemToEquip != null)
                    {
                        weaponItem.gemSockets[i] = supposedlyGemToEquip as GemItem;
                    }
                }
            }
        }
        _serializedItemsInventory.Add(SerializationController._instance.SerializeItem(item));
    }

    public void RemoveItem(Item item)
    {
        itemsInventory.Remove(item);//maybe a little bit too intensive when arrows are removed from inventory?
        SerializeInventory();
    }

    public void DropItem(Item item, bool instatiateDroppedItem = true)
    {
        if (item.itemType == ItemType.Weapon)
        {
            WeaponItem weaponItem = item as WeaponItem;
            for (int i = 0; i < weaponItem.gemSockets.Length; i++)
            {
                if (weaponItem.gemSockets[i] != null)
                {
                    RemoveItem(weaponItem.gemSockets[i]);
                }
            }
            UnequipWeapon(weaponItem);
        }
        else if (item.itemType == ItemType.Gem)
        {
            UnequipGem(item as GemItem);
        }
        else if (item.itemType == ItemType.Ammo)
        {
            List<Item> ammoAvailable = itemsInventory.FindAll(x => x.itemName == item.itemName);//it uses the name instead of the ID because many items can hace the same ID because their ItemsDBB is separate, maybe after all is not so good of an idea to have separate items DBB
            if (ammoAvailable.Count < 1)
            {
                if (ammoSlot.itemID == item.itemID)
                {
                    UnequipAmmo(item as AmmoItem);
                }
            }
        }
        if (item.itemType == ItemType.Bundle)
        {
            ItemsBundle droppedItemsBundle = item as ItemsBundle;
            foreach (var itemInBundle in droppedItemsBundle.itemsBundle)
            {
                itemsInventory.Remove(itemInBundle);
            }
        }
        else
        {
            itemsInventory.Remove(item);
        }

        if (instatiateDroppedItem)
        {
            InstatiateDropedItem(item);
        }

        //_serializedItemsInventory.Remove(SerializationController._instance.SerializeItem(item)); instead of serialializing all the inventory?
        SerializeInventory();
    }

    public void InstatiateDropedItem(Item item)
    {
        GameObject environment = GameObject.Find("Environment");
        GameObject itemInWorld = Instantiate(item.inWorldVersion, transform.position + transform.forward +  new Vector3(0, .5f, 0), transform.rotation, environment.transform);
        if (item.itemType == ItemType.Weapon || item.itemType == ItemType.Trinket)
        {
            itemInWorld.GetComponentInChildren<GemsInObjectsManager>().SpawnGemsInItem(item);
        }        
        itemInWorld.GetComponent<PickableItem>().item = item;//Pass the properties of the inventory item to the scriptable in the inWorld version after a new inWorldVersionPrefab has been instantiated, otherwise it would override the original prefab
        item.inWorldVersion = itemInWorld;
        itemInWorld.GetComponent<PrefabGUID>().uniqueObjectId = item.uniqueItemID;
    }

    public void UnequipWeapon(WeaponItem weaponItem)//TODO: update the anim override controller
    {
        if (weaponsInRightHandSlot[0] == weaponItem)//If the weapon being unequiped is in the weapon equipment slots
        {
            weaponsInRightHandSlot[0] = null;
            GetCorrespondentHandWeaponEquipmentSlot(weaponItem).ClearWeaponItem();

            if (currentRightWeaponIndex == 0)//If the weapon being unequiped is in the hand righ now
            {
                if (_playerManager.isTwoHandingWeapon && backSlotWeapon != weaponItem)//if the player is two handing and the weapon being unequiped is not the back slot weapon //(This can happen because when the player is two handing a weapon if the other hand had a weapon at the moment when it starts being two handed, the other weapon goes to the back slot but remains in the hand at the same time)
                {
                    if (backSlotWeapon != null)//If the backslot is not empty
                    {
                        leftHandWeapon = backSlotWeapon;

                        backSlotWeapon = null;
                        _weaponSlotManager.backSlot.UnloadWeapon();
                        _weaponSlotManager.backSlot.currentWeapon = null;

                        _weaponSlotManager.LoadWeaponOnSlot(leftHandWeapon, true);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                    else//If the back slot is empty
                    {
                        leftHandWeapon = unarmedWeapon;//maybe instead of assigning a unnarmed weapon change the weapon index to -1?
                        _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                }
                _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
                rightHandWeapon = unarmedWeapon;
            }

            _quickSlots.UpdateWeaponQuickSlotsUI(false, unarmedWeapon);
        }

        if (weaponsInRightHandSlot[1] == weaponItem)
        {
            weaponsInRightHandSlot[1] = null;
            GetCorrespondentHandWeaponEquipmentSlot(weaponItem).ClearWeaponItem();

            if (currentRightWeaponIndex == 1)
            {
                if (_playerManager.isTwoHandingWeapon && backSlotWeapon != weaponItem)
                {
                    if (backSlotWeapon != null)
                    {
                        leftHandWeapon = backSlotWeapon;

                        backSlotWeapon = null;
                        _weaponSlotManager.backSlot.UnloadWeapon();
                        _weaponSlotManager.backSlot.currentWeapon = null;

                        _weaponSlotManager.LoadWeaponOnSlot(leftHandWeapon, true);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                    else
                    {
                        leftHandWeapon = unarmedWeapon;
                        _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                }
                _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
                rightHandWeapon = unarmedWeapon;
            }

            _quickSlots.UpdateWeaponQuickSlotsUI(false, unarmedWeapon);
        }

        if (weaponsInLeftHandSlot[0] == weaponItem)
        {
            weaponsInLeftHandSlot[0] = null;
            GetCorrespondentHandWeaponEquipmentSlot(weaponItem).ClearWeaponItem();

            if (currentLeftWeaponIndex == 0)
            {
                if (_playerManager.isTwoHandingWeapon && backSlotWeapon != weaponItem)
                {
                    if (backSlotWeapon != null)
                    {
                        rightHandWeapon = backSlotWeapon;

                        backSlotWeapon = null;
                        _weaponSlotManager.backSlot.UnloadWeapon();
                        _weaponSlotManager.backSlot.currentWeapon = null;

                        _weaponSlotManager.LoadWeaponOnSlot(rightHandWeapon, false);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                    else
                    {
                        rightHandWeapon = unarmedWeapon;
                        _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                }
                _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
                leftHandWeapon = unarmedWeapon;
            }

            _quickSlots.UpdateWeaponQuickSlotsUI(true, unarmedWeapon);
        }

        if (weaponsInLeftHandSlot[1] == weaponItem)
        {
            weaponsInLeftHandSlot[1] = null;
            GetCorrespondentHandWeaponEquipmentSlot(weaponItem).ClearWeaponItem();

            if (currentLeftWeaponIndex == 1)
            {
                if (_playerManager.isTwoHandingWeapon && backSlotWeapon != weaponItem)
                {
                    if (backSlotWeapon != null)
                    {
                        rightHandWeapon = backSlotWeapon;

                        backSlotWeapon = null;
                        _weaponSlotManager.backSlot.UnloadWeapon();
                        _weaponSlotManager.backSlot.currentWeapon = null;

                        _weaponSlotManager.LoadWeaponOnSlot(rightHandWeapon, false);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                    else
                    {
                        rightHandWeapon = unarmedWeapon;
                        _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
                        _playerManager.isTwoHandingWeapon = false;
                    }
                }
                _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
                leftHandWeapon = unarmedWeapon;
            }

            _quickSlots.UpdateWeaponQuickSlotsUI(true, unarmedWeapon);
        }

        if (backSlotWeapon == weaponItem)
        {
            backSlotWeapon = null;
            _weaponSlotManager.backSlot.UnloadWeapon();
            _weaponSlotManager.backSlot.currentWeapon = null;
        }

        weaponItem.isEquiped = false;
    }

    private HandEquipmentSlotUI GetCorrespondentHandWeaponEquipmentSlot(WeaponItem weaponItem)
    {
        HandEquipmentSlotUI[] handEquipmentSlots = _playerManager.UIManager.GetComponentsInChildren<HandEquipmentSlotUI>(true);
        foreach (var handEquipmentSlot in handEquipmentSlots)
        {
            if (handEquipmentSlot.weapon != null)
            {
                if (handEquipmentSlot.weapon.uniqueItemID == weaponItem.uniqueItemID)
                {
                    return handEquipmentSlot;
                }
            }
        }

        return null;
    }

    public void UnequipGem(GemItem gemItem)
    {        
        List<Item> weaponItems = itemsInventory.FindAll(x => x.itemType == ItemType.Weapon);
        foreach (Item weaponItem in weaponItems)
        {
            WeaponItem actualWeapon = weaponItem as WeaponItem;
            for (int i = 0; i < actualWeapon.gemSocketItemID.Length; i++)
            {
                if (actualWeapon.gemSocketItemID[i] == gemItem.uniqueItemID)
                {
                    actualWeapon.gemSocketItemID[i] = "";
                    actualWeapon.gemSockets[i] = null;
                }

                if (rightHandWeapon == actualWeapon)
                {
                    rightHandWeapon.modelPrefab.GetComponentInChildren<GemsInObjectsManager>().SpawnGemsInItem(actualWeapon);
                }

                if (leftHandWeapon == actualWeapon)
                {
                    leftHandWeapon.modelPrefab.GetComponentInChildren<GemsInObjectsManager>().SpawnGemsInItem(actualWeapon);
                }

                if (backSlotWeapon == actualWeapon)
                {
                    backSlotWeapon.modelPrefab.GetComponentInChildren<GemsInObjectsManager>().SpawnGemsInItem(actualWeapon);
                }
            }
        }
        gemItem.isEquiped = false;
    }

    public void UnequipAmmo(AmmoItem ammo)
    {
        HandEquipmentSlotUI[] handEquipmentSlots = _playerManager.UIManager.GetComponentsInChildren<HandEquipmentSlotUI>(true);
        foreach (var handEquipmentSlot in handEquipmentSlots)
        {
            if (handEquipmentSlot.ammoSlot == true)
            {
                handEquipmentSlot.ClearAmmoItem();
            }
        }
        ammoSlot = null;
    }

    public int IsAmmoAvailable()
    {
        if (ammoSlot != null)
        {
            List<Item> ammoAvailable = itemsInventory.FindAll(x => x.itemName == ammoSlot.itemName);
            if (ammoAvailable.Count >= 1)
            {
                return ammoAvailable.Count;
            }
            UnequipAmmo(ammoSlot);
        }
        return 0;
    }

    public void DropItems(DropItemCounter dropItemCounter)
    {
        int cuantity = dropItemCounter.cuantity;

        for (int i = 0; i < itemStacks.Count; i++)
        {
            if (itemStacks[i].Contains(dropedItem))
            {
                if (dropedItem.itemType == ItemType.Ammo || dropedItem.itemType == ItemType.Consumable)
                {
                    if (cuantity == 1)
                    {
                        DropItem(itemStacks[i][cuantity - 1]);//Drops the last item in the stack
                    }
                    else if(cuantity > 1)
                    {
                        ItemsBundle droppedItemsBundle = SerializationController._instance.CreateItemsBundle(dropedItem);
                        List<Item> droppedItemsList = new List<Item>();
                        for (int j = cuantity - 1; j >= 0; j--)
                        {
                            droppedItemsList.Add(itemStacks[i][j]);
                            itemStacks[i].Remove(itemStacks[i][j]);
                        }
                        droppedItemsBundle.itemsBundle = droppedItemsList.ToArray();
                        DropItem(droppedItemsBundle);
                    }
                }
                else
                {
                    for (int j = cuantity - 1; j >= 0; j--)
                    {
                        DropItem(itemStacks[i][j]);
                        itemStacks[i].Remove(itemStacks[i][j]);
                    }
                }

                if (itemStacks[i].Count <= 1)//If there is only 1 object of its kind or none remove it from the stack list
                {
                    itemStacks.RemoveAt(i);
                }
                break;                
            }
        }

        UpdateUIAfterInventoryChange(dropedItem.itemType);
        dropedItem = null;
        SerializeInventory();
    }

    public void DropAllItems()//All items of a kind or a stack, not all items in the inventory
    {
        for (int i = 0; i < itemStacks.Count; i++)
        {
            if (itemStacks[i].Contains(dropedItem))
            {
                if (dropedItem.itemType == ItemType.Ammo || dropedItem.itemType == ItemType.Consumable)
                {
                    ItemsBundle droppedItemsBundle = SerializationController._instance.CreateItemsBundle(dropedItem);
                    List<Item> droppedItemsList = new List<Item>();
                    foreach (Item itemToDrop in itemStacks[i])
                    {
                        droppedItemsList.Add(itemToDrop);
                    }
                    droppedItemsBundle.itemsBundle = droppedItemsList.ToArray();
                    DropItem(droppedItemsBundle);
                }
                else
                {
                    foreach (Item itemToDrop in itemStacks[i])
                    {
                        DropItem(itemToDrop);
                    }
                }
                itemStacks.RemoveAt(i);
                break;
            }
        }
        UpdateUIAfterInventoryChange(dropedItem.itemType);
        dropedItem = null;
        SerializeInventory();
    }

    public void SelectDropedItem(ItemInventorySlot itemInventorySlot)
    {
        if (itemInventorySlot.item.isStackable)
        {
            List<Item> dropItemCuantityInInventory = itemsInventory.FindAll(x => x.itemName == itemInventorySlot.item.itemName);//Change for item ID when stablished note: cant be changed at the moment because each type of items has itw own dbb so their IDs repeat across multiple dbb which lead to item confusion
            if (dropItemCuantityInInventory.Count == 1)
            {
                DropItem(itemInventorySlot.item);
                UpdateUIAfterInventoryChange(itemInventorySlot.item.itemType);
                dropedItem = itemInventorySlot.item;
            }
            else
            {
                dropedItem = itemInventorySlot.item;
                UIManager uIManager = _playerManager.UIManager;
                uIManager.AddOpenWindow(uIManager.ItemDropWindow);
                uIManager.ItemDropWindow.SetActive(true);//Not the best thing to do but will work in the meantime, ideally when clicking the drop button if the item is not stackable the window shouldnt even appear.
            }
        }
        else
        {
            DropItem(itemInventorySlot.item);
            UpdateUIAfterInventoryChange(itemInventorySlot.item.itemType);
        }
    }

    public void SerializeInventory()
    {
        _serializedItemsInventory.Clear();
        foreach (Item item in itemsInventory)
        {
            _serializedItemsInventory.Add(SerializationController._instance.SerializeItem(item));
        }
    }
    public void UpdateUIAfterInventoryChange(ItemType itemType)
    {
        UIManager uIManager = _playerManager.UIManager;
        uIManager.UpdateUI((int)itemType);
    }

    public void HideWeapons()
    {
        if (rightHandWeapon.isUnarmed != true)
        {
            rightHandHiddenWeapon = rightHandWeapon;
            _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
        }

        if (leftHandWeapon.isUnarmed != true)
        {
            leftHandHiddenWeapon = leftHandWeapon;
            _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
        }
    }

    public void UnhideWeapons()
    {
        if (rightHandHiddenWeapon != null)
        {
            rightHandWeapon = rightHandHiddenWeapon;
            _weaponSlotManager.LoadWeaponOnSlot(rightHandWeapon, false);
            rightHandHiddenWeapon = null;
        }

        if (leftHandHiddenWeapon != null)
        {
            leftHandWeapon = leftHandHiddenWeapon;
            _weaponSlotManager.LoadWeaponOnSlot(leftHandWeapon, true);
            leftHandHiddenWeapon = null;
        }
    }

    public void ChangeCurrentWeapon(WeaponHolderSlot weaponHolderSlot)
    {
        if (weaponHolderSlot == null)
            return;

        if (weaponHolderSlot.isLeftHand)
        {
            if (weaponHolderSlot.currentWeapon != null)
            {
                leftHandWeapon = weaponHolderSlot.currentWeapon;
            }
            return;
        }

        if (weaponHolderSlot.isRightHand)
        {
            if (weaponHolderSlot.currentWeapon != null)
            {
                rightHandWeapon = weaponHolderSlot.currentWeapon;
            }
            return;
        }

        if (weaponHolderSlot.isBackSlot)
        {
            backSlotWeapon = weaponHolderSlot.currentWeapon;
            return;
        }
    }
}
