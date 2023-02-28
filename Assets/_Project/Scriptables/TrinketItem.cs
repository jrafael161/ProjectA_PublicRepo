using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Trinket Item")]
public class TrinketItem : Item
{
    public GameObject modelPrefab;

    public bool isEquiped;

    [Header("Gem Sockets")]
    public GemItem[] gemSockets;

    public string[] gemSocketItemID;
}

[System.Serializable]
public class SerializableTrinketItem : SerializableItem
{
    public bool isEquiped;
    public string[] gemSocketItemID;
    public SerializableTrinketItem() : base()
    {
        isEquiped = false;
        gemSocketItemID = new string[0];
    }
    public SerializableTrinketItem(TrinketItem trinketItem) : base(trinketItem)
    {
        isEquiped = trinketItem.isEquiped;
        gemSocketItemID = trinketItem.gemSocketItemID;
    }
    public SerializableTrinketItem(TrinketItem trinketItem, bool PickedUp = false, Transform worldPos = null) : base(trinketItem, PickedUp, worldPos)
    {
        isEquiped = trinketItem.isEquiped;
        gemSocketItemID = trinketItem.gemSocketItemID;
    }
}