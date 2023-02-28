using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GemTypes
{
    Fire,
    Water,
    Electric,
    Rock,
    Wind
}

public enum Quality
{
    Common,
    Wonderful,
    Marvelous,
    Sublime,
    Flawless
}

[CreateAssetMenu(menuName = "Items/Gem Item")]
public class GemItem : Item
{
    public GameObject abilityWarmUpFX;
    public GameObject abilityCastFX;
    public string abiltyAnimation;
    
    public float maxEnergy;
    public float currentEnergy;
    public bool isEquiped;

    [Header("Elemental type")]
    public GemTypes gemType;

    [Header("Gem quality")]
    public Quality gemQuality;

    [Header("Ability description")]
    [TextArea]
    public string abilityDescription;

    public virtual void AttempToUseGemAbility()
    {
        Debug.Log("You atemp to use the gem ability");
    }

    public virtual void SuccessfullyUsedAbility()
    {
        Debug.Log("You used the gem ability");
    }
}

[System.Serializable]
public class SerializableGemItem : SerializableItem
{
    public float maxEnergy;
    public float currentEnergy;
    public bool isEquiped;
    public Quality gemQuality;
    public SerializableGemItem() : base()
    {
        maxEnergy = 0;
        currentEnergy = 0;
        isEquiped = false;
        gemQuality = Quality.Common;
    }
    public SerializableGemItem(GemItem gemItem) : base(gemItem)
    {
        maxEnergy = gemItem.maxEnergy;
        currentEnergy = gemItem.currentEnergy;
        isEquiped = gemItem.isEquiped;
        gemQuality = gemItem.gemQuality;
    }

    public SerializableGemItem(GemItem gemItem, bool PickedUp = false, Transform worldPos = null) : base(gemItem, PickedUp, worldPos)
    {
        maxEnergy = gemItem.maxEnergy;
        currentEnergy = gemItem.currentEnergy;
        isEquiped = gemItem.isEquiped;
        gemQuality = gemItem.gemQuality;
    }
}