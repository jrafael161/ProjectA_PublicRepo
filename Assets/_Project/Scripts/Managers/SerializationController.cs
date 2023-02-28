using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationController : MonoBehaviour
{
    public static SerializationController _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public string SerializeItem(Item item)
    {
        string json;
        switch (item.itemType)
        {
            case ItemType.Consumable:
                //Debug.Log("added a consumable item");
                json = JsonUtility.ToJson(item, true);
                return json;

            case ItemType.KeyItem:
                //Debug.Log("added a keyItem item");
                json = JsonUtility.ToJson(item, true);
                return json;

            case ItemType.Gem:
                SerializableGemItem serializableGemItem = new SerializableGemItem(item as GemItem);
                //Debug.Log("added a gem item" + serializableGemItem.itemName);
                json = JsonUtility.ToJson(serializableGemItem, true);
                return json;

            case ItemType.Weapon:
                SerializableWeaponItem serializableWeaponItem = new SerializableWeaponItem(item as WeaponItem);
                //Debug.Log("added a weapon item" + serializableWeaponItem.itemName);
                json = JsonUtility.ToJson(serializableWeaponItem, true);
                return json;

            case ItemType.Ammo:
                SerializableAmmoItem serializableAmmoItem = new SerializableAmmoItem(item as AmmoItem);
                //Debug.Log("added a weapon item" + serializableAmmoItem.itemName);
                json = JsonUtility.ToJson(serializableAmmoItem, true);
                return json;

            case ItemType.Material:
                SerializableMaterialItem serializableMaterialItem = new SerializableMaterialItem(item as MaterialItem);
                //Debug.Log("added a material item" + serializableMaterialItem.itemName);
                json = JsonUtility.ToJson(serializableMaterialItem, true);
                return json;

            case ItemType.Trinket:
                SerializableTrinketItem serializableTrinketItem = new SerializableTrinketItem(item as TrinketItem);
                //Debug.Log("added a trimket item" + serializableTrinketItem.itemName);
                json = JsonUtility.ToJson(serializableTrinketItem, true);
                //playerInventory._serializedItemsInventory.Add(json);//cannot serialize items like this because the gems they have attached have not been equiped yet
                return json;

            case ItemType.Bundle:
                SerializableItemsBundle serializableItemsBundle = new SerializableItemsBundle(item as ItemsBundle);
                //Debug.Log("added a material item" + serializableMaterialItem.itemName);
                json = JsonUtility.ToJson(serializableItemsBundle, true);
                return json;

            default:
                return "";
        }
    }

    public string SerializeItemInWorld(Item item)
    {
        string json;
        switch (item.itemType)
        {
            case ItemType.Gem:
                SerializableGemItem serializableGemItem = new SerializableGemItem(item as GemItem, item.inWorldVersion.GetComponent<PickableItem>().wasPickedUp, item.inWorldVersion.transform);
                json = JsonUtility.ToJson(serializableGemItem, true);
                return json;

            case ItemType.Weapon:
                WeaponItem weaponItem = item as WeaponItem;
                SerializableWeaponItem serializableWeaponItem = new SerializableWeaponItem(weaponItem, weaponItem.isBeingDisplayed, item.inWorldVersion.GetComponent<PickableItem>().wasPickedUp, item.inWorldVersion.transform);
                json = JsonUtility.ToJson(serializableWeaponItem, true);
                return json;

            case ItemType.Ammo:
                SerializableAmmoItem serializableAmmoItem = new SerializableAmmoItem(item as AmmoItem, item.inWorldVersion.GetComponent<PickableItem>().wasPickedUp, item.inWorldVersion.transform);
                json = JsonUtility.ToJson(serializableAmmoItem, true);
                return json;

            case ItemType.Trinket:
                SerializableTrinketItem serializableTrinketItem = new SerializableTrinketItem(item as TrinketItem, item.inWorldVersion.GetComponent<PickableItem>().wasPickedUp, item.inWorldVersion.transform);
                json = JsonUtility.ToJson(serializableTrinketItem, true);
                return json;

            case ItemType.Bundle:
                ItemsBundle itemsBundleAux = item as ItemsBundle;
                SerializableItemsBundle serializableItemsBundle = new SerializableItemsBundle(itemsBundleAux, item.inWorldVersion.GetComponent<PickableItem>().wasPickedUp, item.inWorldVersion.transform);
                json = JsonUtility.ToJson(serializableItemsBundle, true);
                return json;

            default:
                SerializableItem serializableItem = new SerializableItem(item, item.inWorldVersion.GetComponent<PickableItem>().wasPickedUp, item.inWorldVersion.transform);
                json = JsonUtility.ToJson(serializableItem, true);
                return json;
        }
    }

    public Item DeserializeItemFromSaveFile(string item)
    {
        SerializableItem serializableItem = new SerializableItem();
        JsonUtility.FromJsonOverwrite(item, serializableItem);

        switch (serializableItem.itemType)
        {
            case ItemType.Consumable:
                Item deserializedConsItem = ScriptableObject.CreateInstance<Item>();
                JsonUtility.FromJsonOverwrite(item, deserializedConsItem);
                return deserializedConsItem;

            case ItemType.KeyItem:
                Item deserializedKeyItem = ScriptableObject.CreateInstance<Item>();
                JsonUtility.FromJsonOverwrite(item, deserializedKeyItem);
                return deserializedKeyItem;

            case ItemType.Gem:
                SerializableGemItem serializedGemItem = new SerializableGemItem();

                JsonUtility.FromJsonOverwrite(item, serializedGemItem);
                GemItem gemItemBlank = AssetsDatabaseManager._instance.itemsDatabase.GemsDBB.Find(x => x.itemID == serializedGemItem.itemID);

                GemItem deserializedGemItem = Instantiate(gemItemBlank);

                deserializedGemItem.itemTier = serializedGemItem.itemTier;
                deserializedGemItem.uniqueItemID = serializedGemItem.uniqueItemID;
                deserializedGemItem.maxEnergy = serializedGemItem.maxEnergy;
                deserializedGemItem.currentEnergy = serializedGemItem.currentEnergy;
                deserializedGemItem.isEquiped = serializedGemItem.isEquiped;
                deserializedGemItem.gemQuality = serializedGemItem.gemQuality;

                //if (serializedGemItem.inWorldPosition != Vector3.zero)//How can i move this check to the load item in the saves controller
                //{
                //    GameObject gemItemInWorld = Instantiate(deserializedGemItem.inWorldVersion, serializedGemItem.inWorldPosition, new Quaternion(0, 0, 0, 0));
                //    deserializedGemItem.inWorldVersion = gemItemInWorld;
                //}

                return deserializedGemItem;//Instead of returning the deserialized item return the serializedItem so and depending if the position is Vector3.zero or not spawn it in the world

            case ItemType.Weapon:
                SerializableWeaponItem serializedWeaponItem = new SerializableWeaponItem();

                JsonUtility.FromJsonOverwrite(item, serializedWeaponItem);
                WeaponItem weaponItemBlank = Instantiate(AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB.Find(x => x.itemID == serializedWeaponItem.itemID));

                WeaponItem deserializedWeaponItem = Instantiate(weaponItemBlank);

                deserializedWeaponItem.itemTier = serializedWeaponItem.itemTier;
                deserializedWeaponItem.uniqueItemID = serializedWeaponItem.uniqueItemID;
                deserializedWeaponItem.isEquiped = serializedWeaponItem.isEquiped;
                deserializedWeaponItem.isBeingDisplayed = serializedWeaponItem.isBeingDisplayed;
                deserializedWeaponItem.metal = serializedWeaponItem.metalType;
                deserializedWeaponItem.weaponMovesetID = serializedWeaponItem.weaponMovesetID;

                if (deserializedWeaponItem.weaponMovesetID != weaponItemBlank.weaponMovesetID)//Makes sure to keep the moveset if it was changed
                {
                    WeaponItem deserializedWeaponItemMoveset = AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB.Find(x => x.itemID == serializedWeaponItem.weaponMovesetID);
                    deserializedWeaponItem.weaponAnimController = deserializedWeaponItemMoveset.weaponAnimController;
                }

                deserializedWeaponItem.sharpness = serializedWeaponItem.sharpness;
                deserializedWeaponItem.gemSocketItemID = serializedWeaponItem.gemSocketItemID;

                //if (serializedWeaponItem.inWorldPosition != Vector3.zero)
                //{
                //    GameObject weaponItemInWorld = Instantiate(deserializedWeaponItem.inWorldVersion, serializedWeaponItem.inWorldPosition, new Quaternion(0, 0, 0, 0));
                //    deserializedWeaponItem.inWorldVersion = weaponItemInWorld;
                //}

                return deserializedWeaponItem;

            case ItemType.Ammo:

                SerializableAmmoItem serializedAmmoItem = new SerializableAmmoItem();
                JsonUtility.FromJsonOverwrite(item, serializedAmmoItem);

                AmmoItem ammoItemBlank = AssetsDatabaseManager._instance.itemsDatabase.AmmoTypesDBB.Find(x => x.itemID == serializedAmmoItem.itemID);
                AmmoItem deserializedAmmoItem = Instantiate(ammoItemBlank);

                deserializedAmmoItem.uniqueItemID = serializedAmmoItem.uniqueItemID;
                deserializedAmmoItem.timesUsed = serializedAmmoItem.timesUsed;
                deserializedAmmoItem.isStuck = serializedAmmoItem.isStuck;

                //if (serializedAmmoItem.inWorldPosition != Vector3.zero)
                //{
                //    GameObject ammoItemInWorld = Instantiate(deserializedAmmoItem.inWorldVersion, serializedAmmoItem.inWorldPosition, new Quaternion(0, 0, 0, 0));
                //    deserializedAmmoItem.inWorldVersion = ammoItemInWorld;
                //}

                return deserializedAmmoItem;

            case ItemType.Material:

                SerializableMaterialItem serializedMaterialItem = new SerializableMaterialItem();

                JsonUtility.FromJsonOverwrite(item, serializedMaterialItem);
                MaterialItem materialItemBlank = AssetsDatabaseManager._instance.itemsDatabase.MaterialsDBB.Find(x => x.itemID == serializedMaterialItem.itemID);

                //deserializedMaterialItem.Initialize(deserializedMaterialItemBlank);//Even tough materials doesnt have serializable data when created a new instance the object is blank so it needs to be initialized and the blank cannot be used because all the items would be references to the item in the DBB
                MaterialItem deserializedMaterialItem = Instantiate(materialItemBlank);

                //if (serializedMaterialItem.inWorldPosition != Vector3.zero)
                //{
                //    GameObject materialItemInWorld = Instantiate(deserializedMaterialItem.inWorldVersion, serializedMaterialItem.inWorldPosition, new Quaternion(0, 0, 0, 0));//Needs to be done in order to spawn in world items that are not in origin coordinates
                //    deserializedMaterialItem.inWorldVersion = materialItemInWorld;
                //}

                return deserializedMaterialItem;

            case ItemType.Trinket:

                TrinketItem deserializedTrinketItem = ScriptableObject.CreateInstance<TrinketItem>();
                JsonUtility.FromJsonOverwrite(item, deserializedTrinketItem);
                return deserializedTrinketItem;

            case ItemType.Bundle:

                SerializableItemsBundle serializedItemsBundle = new SerializableItemsBundle();
                JsonUtility.FromJsonOverwrite(item, serializedItemsBundle);

                ItemsBundle itemsBundleBlank = AssetsDatabaseManager._instance.itemsDatabase.BundlesDBB.Find(x => x.itemID == serializedItemsBundle.itemID);
                ItemsBundle deserializedItemsBundle = Instantiate(itemsBundleBlank);

                if (serializedItemsBundle.itemSample.itemType == ItemType.Ammo)
                {
                    deserializedItemsBundle.itemSample = DeserializeItem(serializedItemsBundle.ammoData[0]);
                }
                else if (serializedItemsBundle.itemSample.itemType == ItemType.Ammo)
                {
                    deserializedItemsBundle.itemSample = DeserializeItem(serializedItemsBundle.itemData[0]);
                }

                //ItemsBundle itemsBundle = CreateItemsBundle(deserializedItemsBundle.itemSample);

                if (deserializedItemsBundle.itemSample.itemType == ItemType.Ammo)
                {
                    List<Item> itemsList = new List<Item>();
                    for (int i = 0; i < serializedItemsBundle.ammoData.Length; i++)
                    {
                        itemsList.Add(DeserializeItem(serializedItemsBundle.ammoData[i]));
                    }
                    deserializedItemsBundle.itemsBundle = itemsList.ToArray();
                    deserializedItemsBundle.itemSample = deserializedItemsBundle.itemsBundle[0];
                }
                else if (deserializedItemsBundle.itemSample.itemType == ItemType.Consumable)
                {
                    List<Item> itemsList = new List<Item>();
                    for (int i = 0; i < serializedItemsBundle.itemData.Length; i++)
                    {
                        itemsList.Add(DeserializeItem(serializedItemsBundle.itemData[i]));
                    }
                    deserializedItemsBundle.itemsBundle = itemsList.ToArray();
                    deserializedItemsBundle.itemSample = deserializedItemsBundle.itemsBundle[0];
                }

                //GameObject objectInWorld = Instantiate(deserializedItemsBundle.inWorldVersion, serializedItemsBundle.inWorldPosition, Quaternion.identity);
                //deserializedItemsBundle.inWorldVersion = objectInWorld;
                //if (serializedItemsBundle.inWorldPosition != Vector3.zero)
                //{
                //    GameObject bundleItemInWorld = Instantiate(deserializedItemsBundle.inWorldVersion, serializedItemsBundle.inWorldPosition, new Quaternion(0, 0, 0, 0));//Needs to be done in order to spawn in world items that are not in origin coordinates
                //    deserializedItemsBundle.inWorldVersion = bundleItemInWorld;
                //}

                return deserializedItemsBundle;

            default:
                Item Blankitem = ScriptableObject.CreateInstance<Item>();
                return Blankitem;
        }
    }

    public SerializableItem DeserializeItemFromSaveFile1(string item)
    {
        SerializableItem serializableItem = new SerializableItem();
        JsonUtility.FromJsonOverwrite(item, serializableItem);

        switch (serializableItem.itemType)
        {
            case ItemType.Consumable:
                //ConsumableItem consumableItem = AssetsDatabaseManager._instance.itemsDatabase.ConsumablesDBB.Find(x => x.itemID == serializableItem.itemID);//One way to do it
                return serializableItem;

            case ItemType.KeyItem:
                //Item deserializedKeyItem = ScriptableObject.CreateInstance<Item>();//Another way to do it
                //JsonUtility.FromJsonOverwrite(item, deserializedKeyItem);
                return serializableItem;

            case ItemType.Gem:
                SerializableGemItem serializedGemItem = new SerializableGemItem();

                JsonUtility.FromJsonOverwrite(item, serializedGemItem);
                GemItem gemItemBlank = AssetsDatabaseManager._instance.itemsDatabase.GemsDBB.Find(x => x.itemID == serializedGemItem.itemID);

                GemItem deserializedGemItem = Instantiate(gemItemBlank);

                deserializedGemItem.itemTier = serializedGemItem.itemTier;
                deserializedGemItem.uniqueItemID = serializedGemItem.uniqueItemID;
                deserializedGemItem.maxEnergy = serializedGemItem.maxEnergy;
                deserializedGemItem.currentEnergy = serializedGemItem.currentEnergy;
                deserializedGemItem.isEquiped = serializedGemItem.isEquiped;
                deserializedGemItem.gemQuality = serializedGemItem.gemQuality;

                return serializedGemItem;//Instead of returning the deserialized item return the serializedItem so and depending if the position is Vector3.zero or not spawn it in the world

            case ItemType.Weapon:
                SerializableWeaponItem serializedWeaponItem = new SerializableWeaponItem();

                JsonUtility.FromJsonOverwrite(item, serializedWeaponItem);
                WeaponItem weaponItemBlank = AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB.Find(x => x.itemID == serializedWeaponItem.itemID);

                WeaponItem deserializedWeaponItem = Instantiate(weaponItemBlank);

                deserializedWeaponItem.itemTier = serializedWeaponItem.itemTier;
                deserializedWeaponItem.uniqueItemID = serializedWeaponItem.uniqueItemID;
                deserializedWeaponItem.isEquiped = serializedWeaponItem.isEquiped;
                deserializedWeaponItem.isBeingDisplayed = serializedWeaponItem.isBeingDisplayed;
                deserializedWeaponItem.metal = serializedWeaponItem.metalType;
                deserializedWeaponItem.weaponMovesetID = serializedWeaponItem.weaponMovesetID;

                if (deserializedWeaponItem.weaponMovesetID != weaponItemBlank.weaponMovesetID)//Makes sure to keep the moveset if it was changed
                {
                    WeaponItem deserializedWeaponItemMoveset = AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB.Find(x => x.itemID == serializedWeaponItem.weaponMovesetID);
                    deserializedWeaponItem.weaponAnimController = deserializedWeaponItemMoveset.weaponAnimController;
                }

                deserializedWeaponItem.sharpness = serializedWeaponItem.sharpness;
                deserializedWeaponItem.gemSocketItemID = serializedWeaponItem.gemSocketItemID;

                return serializedWeaponItem;

            case ItemType.Ammo:

                SerializableAmmoItem serializedAmmoItem = new SerializableAmmoItem();
                JsonUtility.FromJsonOverwrite(item, serializedAmmoItem);

                AmmoItem ammoItemBlank = AssetsDatabaseManager._instance.itemsDatabase.AmmoTypesDBB.Find(x => x.itemID == serializedAmmoItem.itemID);
                AmmoItem deserializedAmmoItem = Instantiate(ammoItemBlank);

                deserializedAmmoItem.uniqueItemID = serializedAmmoItem.uniqueItemID;
                deserializedAmmoItem.timesUsed = serializedAmmoItem.timesUsed;
                deserializedAmmoItem.isStuck = serializedAmmoItem.isStuck;

                return serializedAmmoItem;

            case ItemType.Material:

                SerializableMaterialItem serializedMaterialItem = new SerializableMaterialItem();

                JsonUtility.FromJsonOverwrite(item, serializedMaterialItem);
                MaterialItem materialItemBlank = AssetsDatabaseManager._instance.itemsDatabase.MaterialsDBB.Find(x => x.itemID == serializedMaterialItem.itemID);

                //deserializedMaterialItem.Initialize(deserializedMaterialItemBlank);//Even tough materials doesnt have serializable data when created a new instance the object is blank so it needs to be initialized and the blank cannot be used because all the items would be references to the item in the DBB
                MaterialItem deserializedMaterialItem = Instantiate(materialItemBlank);

                return serializedMaterialItem;

            case ItemType.Trinket:
                return serializableItem;

            case ItemType.Bundle:

                SerializableItemsBundle serializedItemsBundle = new SerializableItemsBundle();
                JsonUtility.FromJsonOverwrite(item, serializedItemsBundle);

                ItemsBundle itemsBundleBlank = AssetsDatabaseManager._instance.itemsDatabase.BundlesDBB.Find(x => x.itemID == serializedItemsBundle.itemID);
                ItemsBundle deserializedItemsBundle = Instantiate(itemsBundleBlank);

                if (serializedItemsBundle.itemSample.itemType == ItemType.Ammo)
                {
                    deserializedItemsBundle.itemSample = DeserializeItem(serializedItemsBundle.ammoData[0]);
                }
                else if (serializedItemsBundle.itemSample.itemType == ItemType.Ammo)
                {
                    deserializedItemsBundle.itemSample = DeserializeItem(serializedItemsBundle.itemData[0]);
                }

                //ItemsBundle itemsBundle = CreateItemsBundle(deserializedItemsBundle.itemSample);

                if (deserializedItemsBundle.itemSample.itemType == ItemType.Ammo)
                {
                    List<Item> itemsList = new List<Item>();
                    for (int i = 0; i < serializedItemsBundle.ammoData.Length; i++)
                    {
                        itemsList.Add(DeserializeItem(serializedItemsBundle.ammoData[i]));
                    }
                    deserializedItemsBundle.itemsBundle = itemsList.ToArray();
                    deserializedItemsBundle.itemSample = deserializedItemsBundle.itemsBundle[0];
                }
                else if (deserializedItemsBundle.itemSample.itemType == ItemType.Consumable)
                {
                    List<Item> itemsList = new List<Item>();
                    for (int i = 0; i < serializedItemsBundle.itemData.Length; i++)
                    {
                        itemsList.Add(DeserializeItem(serializedItemsBundle.itemData[i]));
                    }
                    deserializedItemsBundle.itemsBundle = itemsList.ToArray();
                    deserializedItemsBundle.itemSample = deserializedItemsBundle.itemsBundle[0];
                }

                return serializedItemsBundle;

            default:
                return serializableItem;
        }
    }

    public Item DeserializeItem(SerializableItem serializedItem)
    {
        switch (serializedItem.itemType)
        {
            case ItemType.Consumable:
                Item deserializedConsItem = ScriptableObject.CreateInstance<Item>();
                return deserializedConsItem;

            case ItemType.KeyItem:
                Item deserializedKeyItem = ScriptableObject.CreateInstance<Item>();
                return deserializedKeyItem;

            case ItemType.Gem:

                GemItem gemItemBlank = AssetsDatabaseManager._instance.itemsDatabase.GemsDBB.Find(x => x.itemID == serializedItem.itemID);
                GemItem deserializedGemItem = Instantiate(gemItemBlank);
                SerializableGemItem serializableGemItem = serializedItem as SerializableGemItem;

                deserializedGemItem.itemTier = serializableGemItem.itemTier;
                deserializedGemItem.uniqueItemID = serializableGemItem.uniqueItemID;
                deserializedGemItem.maxEnergy = serializableGemItem.maxEnergy;
                deserializedGemItem.currentEnergy = serializableGemItem.currentEnergy;
                deserializedGemItem.isEquiped = serializableGemItem.isEquiped;
                deserializedGemItem.gemQuality = serializableGemItem.gemQuality;

                return deserializedGemItem;

            case ItemType.Weapon:

                WeaponItem weaponItemBlank = AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB.Find(x => x.itemID == serializedItem.itemID);
                WeaponItem deserializedWeaponItem = Instantiate(weaponItemBlank);
                SerializableWeaponItem serializedWeaponItem = serializedItem as SerializableWeaponItem;

                deserializedWeaponItem.itemTier = serializedWeaponItem.itemTier;
                deserializedWeaponItem.uniqueItemID = serializedWeaponItem.uniqueItemID;
                deserializedWeaponItem.isEquiped = serializedWeaponItem.isEquiped;
                deserializedWeaponItem.isBeingDisplayed = serializedWeaponItem.isBeingDisplayed;
                deserializedWeaponItem.metal = serializedWeaponItem.metalType;
                deserializedWeaponItem.weaponMovesetID = serializedWeaponItem.weaponMovesetID;

                if (deserializedWeaponItem.weaponMovesetID != weaponItemBlank.weaponMovesetID)//Makes sure to keep the moveset if it was changed
                {
                    WeaponItem deserializedWeaponItemMoveset = AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB.Find(x => x.itemID == serializedWeaponItem.weaponMovesetID);
                    deserializedWeaponItem.weaponAnimController = deserializedWeaponItemMoveset.weaponAnimController;
                }

                deserializedWeaponItem.sharpness = serializedWeaponItem.sharpness;
                deserializedWeaponItem.gemSocketItemID = serializedWeaponItem.gemSocketItemID;

                return deserializedWeaponItem;

            case ItemType.Ammo:

                AmmoItem ammoItemBlank = AssetsDatabaseManager._instance.itemsDatabase.AmmoTypesDBB.Find(x => x.itemID == serializedItem.itemID);
                AmmoItem deserializedAmmoItem = Instantiate(ammoItemBlank);
                SerializableAmmoItem serializedAmmoItem = serializedItem as SerializableAmmoItem;

                if (serializedAmmoItem.uniqueItemID == "" || serializedAmmoItem.uniqueItemID == null)
                {
                    deserializedAmmoItem.uniqueItemID = System.Guid.NewGuid().ToString();
                }
                else
                {
                    deserializedAmmoItem.uniqueItemID = serializedAmmoItem.uniqueItemID;
                }
                
                deserializedAmmoItem.timesUsed = serializedAmmoItem.timesUsed;
                deserializedAmmoItem.isStuck = serializedAmmoItem.isStuck;
                deserializedAmmoItem.inWorldVersion.transform.position = serializedAmmoItem.inWorldPosition;

                return deserializedAmmoItem;

            case ItemType.Material:

                MaterialItem serializedMaterialItem = ScriptableObject.CreateInstance<MaterialItem>();

                MaterialItem materialItemBlank = AssetsDatabaseManager._instance.itemsDatabase.MaterialsDBB.Find(x => x.itemID == serializedMaterialItem.itemID);

                //deserializedMaterialItem.Initialize(deserializedMaterialItemBlank);//Even tough materials doesnt have serializable data when created a new instance the object is blank so it needs to be initialized and cannot be used the blank because all the items would be references to the item in the DBB
                MaterialItem deserializedMaterialItem = Instantiate(materialItemBlank);

                return deserializedMaterialItem;

            case ItemType.Trinket:
                Item deserializedTrinketItem = ScriptableObject.CreateInstance<Item>();
                return deserializedTrinketItem;

            case ItemType.Bundle:
                Debug.LogWarning("Bundles are not supposed to be serialized this way");
                return null;

            default:
                Item Blankitem = ScriptableObject.CreateInstance<Item>();
                return Blankitem;
        }
    }

    public ItemsBundle CreateItemsBundle(Item item)
    {
        ItemsBundle itemsBundle = null;
        if (item.itemType == ItemType.Ammo)
        {
            AmmoItem ammoItem = item as AmmoItem;
            ItemsBundle bundleItemIdentifier = ammoItem.InWorldBundleVersion.GetComponent<PickableItem>().item as ItemsBundle;
            ItemsBundle itemsBundleBlank = AssetsDatabaseManager._instance.itemsDatabase.BundlesDBB.Find(x => x.itemID == bundleItemIdentifier.itemID);
            itemsBundle = Instantiate(itemsBundleBlank);
        }
        else if (item.itemType == ItemType.Consumable)
        {
            ConsumableItem consumableItem = item as ConsumableItem;
            ItemsBundle bundleItemIdentifier = consumableItem.InWorldBundleVersion.GetComponent<PickableItem>().item as ItemsBundle;
            ItemsBundle itemsBundleBlank = AssetsDatabaseManager._instance.itemsDatabase.BundlesDBB.Find(x => x.itemID == bundleItemIdentifier.itemID);
            itemsBundle = Instantiate(itemsBundleBlank);
        }
        return itemsBundle;
    }

    public Item CreateItemForInventory(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Consumable:
                ConsumableItem consumableItem = Instantiate(item) as ConsumableItem;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    consumableItem.uniqueItemID = System.Guid.NewGuid().ToString();

                ConsumableItem consumableItemBlank = AssetsDatabaseManager._instance.itemsDatabase.ConsumablesDBB.Find(x => x.itemID == consumableItem.itemID);
                consumableItem.inWorldVersion = consumableItemBlank.inWorldVersion;
                return consumableItem;

            case ItemType.KeyItem:
                KeyItem keyItem = Instantiate(item) as KeyItem;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    keyItem.uniqueItemID = System.Guid.NewGuid().ToString();

                KeyItem keyItemBlank = AssetsDatabaseManager._instance.itemsDatabase.KeyItemsDBB.Find(x => x.itemID == keyItem.itemID);
                keyItem.inWorldVersion = keyItemBlank.inWorldVersion;
                return keyItem;

            case ItemType.Gem:
                GemItem gemItem = Instantiate(item) as GemItem;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    gemItem.uniqueItemID = System.Guid.NewGuid().ToString();

                GemItem gemItemBlank = AssetsDatabaseManager._instance.itemsDatabase.GemsDBB.Find(x => x.itemID == gemItem.itemID);
                gemItem.inWorldVersion = gemItemBlank.inWorldVersion;
                return gemItem;

            case ItemType.Weapon:
                WeaponItem weaponItem = Instantiate(item) as WeaponItem;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    weaponItem.uniqueItemID = System.Guid.NewGuid().ToString();

                WeaponItem weaponItemBlank = AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB.Find(x => x.itemID == weaponItem.itemID);
                weaponItem.inWorldVersion = weaponItemBlank.inWorldVersion;
                return weaponItem;

            case ItemType.Ammo:
                AmmoItem ammoItem = Instantiate(item) as AmmoItem;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    ammoItem.uniqueItemID = System.Guid.NewGuid().ToString();

                AmmoItem ammoItemBlank = AssetsDatabaseManager._instance.itemsDatabase.AmmoTypesDBB.Find(x => x.itemID == ammoItem.itemID);
                ammoItem.inWorldVersion = ammoItemBlank.inWorldVersion;
                return ammoItem;

            case ItemType.Material:
                MaterialItem materialItem = Instantiate(item) as MaterialItem;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    materialItem.uniqueItemID = System.Guid.NewGuid().ToString();

                MaterialItem materialItemBlank = AssetsDatabaseManager._instance.itemsDatabase.MaterialsDBB.Find(x => x.itemID == materialItem.itemID);
                materialItem.inWorldVersion = materialItemBlank.inWorldVersion;
                return materialItem;

            case ItemType.Trinket:
                TrinketItem trinketItem = Instantiate(item) as TrinketItem;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    trinketItem.uniqueItemID = System.Guid.NewGuid().ToString();

                TrinketItem trinketItemBlank = AssetsDatabaseManager._instance.itemsDatabase.TrinketsDBB.Find(x => x.itemID == trinketItem.itemID);
                trinketItem.inWorldVersion = trinketItemBlank.inWorldVersion;
                return trinketItem;

            case ItemType.Bundle:
                ItemsBundle itemsBundle = Instantiate(item) as ItemsBundle;

                if (CheckIfItemFromScene(item.inWorldVersion.transform))
                    itemsBundle.uniqueItemID = System.Guid.NewGuid().ToString();

                ItemsBundle itemsBundleBlank = AssetsDatabaseManager._instance.itemsDatabase.BundlesDBB.Find(x => x.itemID == itemsBundle.itemID);
                itemsBundle.inWorldVersion = itemsBundleBlank.inWorldVersion;
                return itemsBundle;

            default:
                return null;
        }
    }

    public bool CheckIfItemFromScene(Transform itemOriginalPos)
    {
        if (itemOriginalPos.position == Vector3.zero)//It means it was an object from the scene and not dropped by the player or some entity in game
        {
            return true;
        }
        return false;
    }
}
