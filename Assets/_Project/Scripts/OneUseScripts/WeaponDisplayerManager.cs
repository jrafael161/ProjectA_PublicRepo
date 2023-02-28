using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDisplayerManager : MonoBehaviour, ISaveDataPersistance
{
    [SerializeField]
    GameObject _objectInventoryWindow;
    public GameObject WeaponPivot { get => _weaponPivot; set => _weaponPivot = value; } [SerializeField] GameObject _weaponPivot;
    public PlayerManager InteractingPlayer { get => _interactingPlayer; set => _interactingPlayer = value; }

    public UnityEngine.UI.Image WeaponSprite;

    [SerializeField] PlayerManager _interactingPlayer;
    public void OpenWeaponSlot(PlayerManager playerInteracting)
    {
        UpdateWeaponSprite();
        _objectInventoryWindow.SetActive(true);

        _interactingPlayer = playerInteracting;
        UIManager uIManager = playerInteracting.UIManager;
        uIManager.AddOpenWindow(_objectInventoryWindow);//Maybe move to being an event?
        InputHandler inputHandler = playerInteracting.gameObject.GetComponent<InputHandler>();
        inputHandler.OpenUI();//Maybe move to being an event?
    }

    public void DisplayObject(WeaponItem weaponItem)
    {
        RetrieveDisplayedWeapon();
        weaponItem.isBeingDisplayed = true;
        InstatiateDisplayedWeapon(weaponItem);
    }

    private void InstatiateDisplayedWeapon(WeaponItem weaponItem)
    {
        WeaponSprite.sprite = weaponItem.itemIcon;

        GameObject itemInWorld = Instantiate(weaponItem.inWorldVersion, WeaponPivot.transform.position, WeaponPivot.transform.rotation, WeaponPivot.transform);
        itemInWorld.GetComponent<Rigidbody>().useGravity = false;
        itemInWorld.GetComponent<Rigidbody>().isKinematic = true;

        itemInWorld.GetComponentInChildren<GemsInObjectsManager>().SpawnGemsInItem(weaponItem);
        itemInWorld.GetComponent<PickableItem>().item = weaponItem;//Pass the properties of the inventory item to the scriptable in the inWorld version after a new inWorldVersionPrefab has been instantiated, otherwise it would override the original prefab
        weaponItem.inWorldVersion = itemInWorld;
        itemInWorld.GetComponent<PrefabGUID>().uniqueObjectId = weaponItem.uniqueItemID;

        PlayerInventory playerInventory = _interactingPlayer.gameObject.GetComponent<PlayerInventory>();
        playerInventory.DropItem(weaponItem, false);
    }

    public void RetrieveDisplayedWeapon()
    {
        if (WeaponPivot.GetComponentInChildren<PickableItem>() != null)
        {
            PlayerInventory playerInventory = _interactingPlayer.gameObject.GetComponent<PlayerInventory>();

            WeaponItem weaponItem = WeaponPivot.GetComponentInChildren<PickableItem>().item as WeaponItem;
            weaponItem.isBeingDisplayed = false;
            playerInventory.AddItem(weaponItem);
            GameObject inWorldmodel = AssetsDatabaseManager._instance.itemsDatabase.WeaponsDBB[weaponItem.itemID].inWorldVersion;//This only works if the itemID is the same as the item index in the DBB, also could I get away instanciating the same weapon and just passing the weapon properties?
            GameObject WeaponModelDisplayed = weaponItem.inWorldVersion;
            DestroyImmediate(WeaponModelDisplayed);//So the wepon interaction can be dispatched and the sapite changed in the same frame otherwise due to how is handled the weapon detection the collider is still there when is cheked
            weaponItem.inWorldVersion = inWorldmodel;
        }
        UpdateWeaponSprite();
    }

    private void UpdateWeaponSprite()
    {
        if (_weaponPivot.GetComponentInChildren<PickableItem>() != null)
        {
            WeaponSprite.sprite = _weaponPivot.GetComponentInChildren<PickableItem>().item.itemIcon;
        }
        else
        {
            WeaponSprite.sprite = null;
        }
    }
}