using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Material Item")]
public class MaterialItem : Item
{

}


[System.Serializable]
public class SerializableMaterialItem : SerializableItem
{
    public SerializableMaterialItem() : base()
    {
    }
    public SerializableMaterialItem(MaterialItem materialItem) : base(materialItem)
    {
    }
}
