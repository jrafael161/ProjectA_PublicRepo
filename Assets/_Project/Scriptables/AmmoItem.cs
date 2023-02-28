using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ammo Item")]
public class AmmoItem : Item
{
    public MetalType metalTip;
    public int timesUsed;//With more times used theres a higher probability that the arrow breaks upon usage (when it hits the opponent)
    public float forwardVelocity;
    public float upwardVelocity;
    public float ammoMass;
    public bool useGravity;
    public int damage;
    public bool isStuck;

    public GameObject InWorldBundleVersion;//Used for creating ammo bundles

    public GameObject loadItemModel;//Used for the animations involving the bow and arrow
    public GameObject damagingModel;//The model that will deal the damage
    public GameObject stuckModel;//The model that instatiates at colliding with an object
}

[System.Serializable]
public class SerializableAmmoItem : SerializableItem
{
    public int timesUsed;
    public bool isStuck;

    public SerializableAmmoItem() : base()
    {
        timesUsed = 0;
        isStuck = false;
    }
    public SerializableAmmoItem(AmmoItem ammoItem) : base(ammoItem)
    {
        timesUsed = ammoItem.timesUsed;
        isStuck = ammoItem.isStuck;
    }

    public SerializableAmmoItem(AmmoItem ammoItem, bool PickedUp = false, Transform worldPos = null) : base(ammoItem, PickedUp, worldPos)
    {
        timesUsed = ammoItem.timesUsed;
        isStuck = ammoItem.isStuck;
    }
}