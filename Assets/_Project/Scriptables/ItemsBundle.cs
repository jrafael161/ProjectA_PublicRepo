using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/itemsBundle")]
public class ItemsBundle : Item
{
    public Item itemSample;
    public Item[] itemsBundle;
}

[System.Serializable]
public class SerializableItemsBundle : SerializableItem
{
    public SerializableItem itemSample;
    public SerializableAmmoItem[] ammoData;
    public SerializableItem[] itemData;

    public SerializableItemsBundle() : base()
    {
        itemSample = null;
        ammoData = new SerializableAmmoItem[0];
        itemData = new SerializableItem[0];
    }
    public SerializableItemsBundle(ItemsBundle itemsBundle) : base(itemsBundle)//there is no case where a bundle doesnt have a transdorm different from Vector3.zero because they only exist in world
    {
        if (itemsBundle.itemsBundle[0].itemType == ItemType.Ammo)
        {
            ammoData = new SerializableAmmoItem[itemsBundle.itemsBundle.Length];
            for (int i = 0; i < itemsBundle.itemsBundle.Length; i++)
            {
                ammoData[i] = new SerializableAmmoItem(itemsBundle.itemsBundle[i] as AmmoItem);
            }
            itemSample = ammoData[0];
        }
        else if (itemsBundle.itemsBundle[0].itemType == ItemType.Consumable)
        {
            itemData = new SerializableItem[itemsBundle.itemsBundle.Length];
            for (int i = 0; i < itemsBundle.itemsBundle.Length; i++)
            {
                itemData[i] = new SerializableItem(itemsBundle.itemsBundle[i]);
            }
            itemSample = itemData[0];
        }
    }

    public SerializableItemsBundle(ItemsBundle itemsBundle, bool PickedUp = false, Transform worldPos = null) : base(itemsBundle, PickedUp, worldPos)
    {
        if (itemsBundle.itemsBundle[0].itemType == ItemType.Ammo)
        {
            ammoData = new SerializableAmmoItem[itemsBundle.itemsBundle.Length];
            for (int i = 0; i < itemsBundle.itemsBundle.Length; i++)
            {
                ammoData[i] = new SerializableAmmoItem(itemsBundle.itemsBundle[i] as AmmoItem);
            }
            itemSample = ammoData[0];
        }
        else if (itemsBundle.itemsBundle[0].itemType == ItemType.Consumable)
        {
            itemData = new SerializableItem[itemsBundle.itemsBundle.Length];
            for (int i = 0; i < itemsBundle.itemsBundle.Length; i++)
            {
                itemData[i] = new SerializableItem(itemsBundle.itemsBundle[i]);
            }
            itemSample = itemData[0];
        }
    }
}