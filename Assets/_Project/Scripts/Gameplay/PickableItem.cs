using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PickableItem : MonoBehaviour, ISaveDataPersistance
{
    //public string interactableText;
    public ItemType itemTypes;

    public bool wasPickedUp;
    public Item item;

    private void Start()
    {
        item = Instantiate(item);//When an item placed in scene is created it must create a new intance of the item to avoid overriding the original scriptable object
        if (item.uniqueItemID == "")
        {
            item.uniqueItemID = GetComponent<PrefabGUID>().uniqueObjectId;
        }
    }

    public string LoadData(SaveData saveData, string itemID)
    {
        itemID = "";
        string tempSerializedItem = null;
        saveData._serializedWorldItems.TryGetValue(item.uniqueItemID, out tempSerializedItem);
        if (tempSerializedItem != null)
        {
            SerializableItem serializableItem = JsonUtility.FromJson<SerializableItem>(tempSerializedItem);
            wasPickedUp = serializableItem.wasPickedUp;
            gameObject.SetActive(!serializableItem.wasPickedUp);
            itemID = item.uniqueItemID;
        }
        return itemID;
    }

    public void StoreData(ref SaveData saveData)
    {
        string json = "";
        Transform inWorldPrefabPosition = item.inWorldVersion.transform;//It passes the position of the in worl version and not its own transform in order to keep items placed in scene from overriding the original inWorldVersion prefab

        switch (item.itemType)
        {
            case ItemType.Gem:
                
                SerializableGemItem serializableGemItem = new SerializableGemItem(item as GemItem, wasPickedUp, inWorldPrefabPosition);
                json = JsonUtility.ToJson(serializableGemItem, true);
                break;

            case ItemType.Weapon:
                WeaponItem weaponItem = item as WeaponItem;
                SerializableWeaponItem serializableWeaponItem = new SerializableWeaponItem(weaponItem, weaponItem.isBeingDisplayed, wasPickedUp, inWorldPrefabPosition);
                json = JsonUtility.ToJson(serializableWeaponItem, true);
                break;

            case ItemType.Ammo:
                SerializableAmmoItem serializableAmmoItem = new SerializableAmmoItem(item as AmmoItem, wasPickedUp, inWorldPrefabPosition);
                json = JsonUtility.ToJson(serializableAmmoItem, true);
                break;

            case ItemType.Trinket:
                SerializableTrinketItem serializableTrinketItem = new SerializableTrinketItem(item as TrinketItem, wasPickedUp, inWorldPrefabPosition);
                json = JsonUtility.ToJson(serializableTrinketItem, true);
                break;

            case ItemType.Bundle:
                SerializableItemsBundle serializableItemsBundle = new SerializableItemsBundle(item as ItemsBundle, wasPickedUp, inWorldPrefabPosition);
                json = JsonUtility.ToJson(serializableItemsBundle, true);
                break;

            default:
                SerializableItem serializableItem = new SerializableItem(item, wasPickedUp, inWorldPrefabPosition);
                json = JsonUtility.ToJson(serializableItem, true);
                break;
        }

        if (item.uniqueItemID == "")
        {
            item.uniqueItemID = GetComponent<PrefabGUID>().uniqueObjectId;
        }
        else if (saveData._serializedWorldItems.ContainsKey(item.uniqueItemID))
        {
            saveData._serializedWorldItems.Remove(item.uniqueItemID);//removes the object to add it again in case there has been changes in its propertys, maybe i culd check before deleting it if it has changed so in the case it has not changed i leave it as it is
        }

        saveData._serializedWorldItems.Add(item.uniqueItemID, json);
    }

    public void PickUpItem(PlayerManager interactingPlayer)
    {
        interactingPlayer.gameObject.GetComponent<PlayerMovementController>().playerRigidbody.velocity = Vector3.zero; //Stops the player whilst moving when picking up item
        interactingPlayer.gameObject.GetComponentInChildren<PlayerAnimController>().playTargetAnimation("PickUpItem", false);//Plays animation of looting the item

        PlayerInteraction playerInteraction = interactingPlayer.gameObject.GetComponentInChildren<PlayerInteraction>();

        int pickUpItemIndex = playerInteraction.pickableObjects.IndexOf(this);
        Item pickedUpItem = SerializationController._instance.CreateItemForInventory(playerInteraction.pickableObjects[pickUpItemIndex].item);

        if (item.inWorldVersion.transform.position == Vector3.zero)//Its zero because the prefab position is vector3.zero therefore is an object placed in the world
        {
            wasPickedUp = true;
            playerInteraction.interactables.Remove(playerInteraction.interactables.Find(x => x.gameObject == gameObject));
            gameObject.SetActive(false);
        }
        else
        {
            //if (SavesController._instance.dataPersistanceObjects.Contains(this))//If the object for some reason was not in the persistence object pool leave it as it is so when added the player can interact with it properly
            //{
            //    SavesController._instance.dataPersistanceObjects.Remove(this);
            //    Destroy(gameObject);
            //}
            playerInteraction.interactables.Remove(playerInteraction.interactables.Find(x => x.gameObject == gameObject));
            Destroy(gameObject);
        }

        interactingPlayer.gameObject.GetComponent<PlayerInventory>().AddItem(pickedUpItem);
        
        interactingPlayer.UIManager.itemAdquisitionMessagePrompt.GetComponentInChildren<TMP_Text>().text = "Added " + playerInteraction.pickableObjects[pickUpItemIndex].item.itemName + " to inventory";
        interactingPlayer.UIManager.itemAdquisitionMessagePrompt.SetActive(true);
        playerInteraction.StartDisableMessageCoroutine();

        playerInteraction.pickableObjects.RemoveAt(pickUpItemIndex);

        if (playerInteraction.interactables.Count <= 1)
        {
            interactingPlayer.UIManager.interactionMessageTogglePrompt.SetActive(false);
        }
    }

    public void EnablePickup()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<InteractionIdentifier>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        this.enabled = true;
    }

    public void DisablePickup()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<InteractionIdentifier>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        this.enabled = false;
    }
}
