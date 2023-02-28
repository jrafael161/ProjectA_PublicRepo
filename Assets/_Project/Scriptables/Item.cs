using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Consumable,
    KeyItem,
    Gem,
    Weapon,
    Ammo,
    Material,
    Trinket,
    Bundle
}

public enum ItemTier
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{    
    [Header("Item Information")]
    public Sprite itemIcon;

    public string itemName;
    public int itemID;
    public ItemType itemType;
    public ItemTier itemTier;

    public string uniqueItemID;

    public bool isStackable;

    [TextArea]
    public string itemDescription;

    public GameObject inWorldVersion;
    public string _serializedItem;
}

[System.Serializable]
public class SerializableItem
{
    public string itemName;
    public int itemID;
    public ItemType itemType;
    public ItemTier itemTier;
    public string uniqueItemID;

    public Vector3 inWorldPosition;
    public bool wasPickedUp;

    public SerializableItem()
    {
        itemName = "";
        itemID = 0;
        itemType = ItemType.Consumable;
        itemTier = ItemTier.Common;
        uniqueItemID = "";
        inWorldPosition = new Vector3(-1, -1, -1);
        wasPickedUp = false;
    }
    public SerializableItem(Item item, bool PickedUp = false, Transform worldPos=null)
    {
        itemName = item.itemName;
        itemID = item.itemID;
        itemType = item.itemType;
        itemTier = item.itemTier;
        uniqueItemID = item.uniqueItemID;
        wasPickedUp = PickedUp;
        if (worldPos != null)
        {
            inWorldPosition = worldPos.position;
        }        
    }
}

