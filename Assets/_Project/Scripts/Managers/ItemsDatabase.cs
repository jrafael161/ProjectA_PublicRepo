using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemsDatabase
{
    public List<WeaponItem> WeaponsDBB;
    public List<AmmoItem> AmmoTypesDBB;
    public List<GemItem> GemsDBB;
    public List<KeyItem> KeyItemsDBB;
    public List<ConsumableItem> ConsumablesDBB;
    public List<MaterialItem> MaterialsDBB;
    public List<TrinketItem> TrinketsDBB;
    public List<ItemsBundle> BundlesDBB;

    /*
    public WeaponItem WeaponBlank;
    public GemItem GemBlank;
    public KeyItem KeyItemBlank;
    public ConsumableItem ConsumableBlank;
    public MaterialItem MaterialBlank;
    public TrinketItem TrinketBlank;
    */

    private void Awake()//Pass to constructor if doesnt belongs to monobehaviour
    {
        /*
        WeaponBlank = ScriptableObject.CreateInstance<WeaponItem>();
        GemBlank = ScriptableObject.CreateInstance<GemItem>();
        KeyItemBlank = ScriptableObject.CreateInstance<KeyItem>();
        ConsumableBlank = ScriptableObject.CreateInstance<ConsumableItem>();
        MaterialBlank = ScriptableObject.CreateInstance<MaterialItem>();
        TrinketBlank = ScriptableObject.CreateInstance<TrinketItem>();
        */
    }
    public void CreateItemDatabase()
    {
        /*
        FillDatabase(ItemType.Consumable);
        FillDatabase(ItemType.Gem);
        FillDatabase(ItemType.KeyItem);
        FillDatabase(ItemType.Material);
        FillDatabase(ItemType.Trinket);
        FillDatabase(ItemType.Weapon);
        */
    }

    private void FillDatabase(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Consumable:
            //#if UNITY_EDITOR
                for (int i = 0; i < 1; i++)
                {
                    ConsumableItem scriptableObject = Resources.Load("Scriptables/Items/Consumables/Consumable_" + i) as ConsumableItem;
                    ConsumablesDBB.Add(scriptableObject);
                }
            //#endif
                /*
            #if UNITY_STANDALONE_WIN
                for (int i = 0; i < 1; i++)
                {
                    Resources.Load(Application.dataPath + "/Scriptables/Items/Consumables/Consumable_" + i);
                }
            #endif
                */
                break;
            case ItemType.KeyItem:
                break;
            case ItemType.Gem:
                for (int i = 0; i < 6; i++)
                {
                    GemItem scriptableObject = Resources.Load("Scriptables/Items/Gems/Gem_" + i) as GemItem;
                    GemsDBB.Add(scriptableObject);
                }
                break;
            case ItemType.Weapon:
                break;
            case ItemType.Material:
                break;
            case ItemType.Trinket:
                break;
            default:
                break;
        }
    }

    public GemItem GetGemItem(int itemID)
    {
        return GemsDBB[itemID];
    }
}
