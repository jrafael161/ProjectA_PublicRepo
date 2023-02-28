using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MetalType
{
    Copper,
    Tin,    
    Lead,
    Iron,
    Meteorite,
    Silver,
    Gold,
    Platinum,
    Tungsten,
    Titanium,
    Mythtril,
    Adamantite    
}

public enum WeaponType
{
    Sword,
    Axe,
    Bow,
    Mace,
    Spear,
    Fist
}

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public WeaponType weaponType;

    public GameObject modelPrefab;
    public bool isUnarmed;//Have to change the way unarmed weapons work

    public bool isEquiped;
    public bool isBeingDisplayed;

    public MetalType metal;

    public float weaponRange;
    public int weaponMovesetID;
    public AnimatorOverrideController weaponAnimController;

    [Header("Weapon Damage")]
    public int sharpness;//0-100 it does less damage according to the sharpeness
    public int weaponDamage;

    [Header("Stamina Cost")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;

    [Header("Gem Sockets")]
    public GemItem[] gemSockets;

    public string[] gemSocketItemID;
}

[System.Serializable]
public class SerializableWeaponItem : SerializableItem
{
    public bool isEquiped;
    public bool isBeingDisplayed;
    public MetalType metalType;
    public int weaponMovesetID;
    public string[] gemSocketItemID;
    public int weaponDamage;
    public int sharpness;
    public SerializableWeaponItem() : base()
    {
        isEquiped = false;
        isBeingDisplayed = false;
        metalType = MetalType.Copper;
        weaponMovesetID = itemID;
        gemSocketItemID = new string[0];
        weaponDamage = 0;
        sharpness = 0;
    }
    public SerializableWeaponItem(WeaponItem weaponItem) : base(weaponItem)
    {
        isEquiped = weaponItem.isEquiped;
        isBeingDisplayed = weaponItem.isBeingDisplayed;
        metalType = weaponItem.metal;
        weaponMovesetID = weaponItem.weaponMovesetID;
        sharpness = weaponItem.sharpness;
        weaponDamage = 0;
        gemSocketItemID = weaponItem.gemSocketItemID;
    }

    public SerializableWeaponItem(WeaponItem weaponItem, bool isDisplayed = false, bool PickedUp = false, Transform worldPos = null) : base(weaponItem, PickedUp, worldPos)
    {
        isEquiped = weaponItem.isEquiped;
        isBeingDisplayed = isDisplayed;
        metalType = weaponItem.metal;
        weaponMovesetID = weaponItem.weaponMovesetID;
        sharpness = weaponItem.sharpness;
        weaponDamage = 0;
        gemSocketItemID = weaponItem.gemSocketItemID;
    }
}
