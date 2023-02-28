using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class GemsInObjectsManager : MonoBehaviour
{
    public GameObject[] GemSlots;//The actual gem model
    public GameObject[] GemSockets;//The parent object that serves as the position to instatiat the gem
    public GemItem[] EquipedGems;//The reference to the gem being equiped
    MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    public void SpawnGemsInItem(Item holdItem)
    {
        switch (holdItem.itemType)
        {
            case ItemType.Weapon:
                ManageGemInWeapon(holdItem as WeaponItem);
                break;
            case ItemType.Trinket:
                SpawnGemInTrinket(holdItem as TrinketItem);
                break;
            default:
                break;
        }      
    }

    public void ManageGemInWeapon(WeaponItem weaponItem)
    {
        for (int i = 0; i < weaponItem.gemSockets.Length; i++)
        {
            if (weaponItem.gemSockets[i] != null)
            {
                if (GemSlots[i] != null)//If its null it should spawn the gem model and then procede with the material assignation
                {
                    GemSlots[i].SetActive(true);
                    Renderer gemRenderer = GemSlots[i].GetComponent<Renderer>();
                    Renderer gemStandardMat = AssetsDatabaseManager._instance.itemsDatabase.GemsDBB.Find(x => x.itemID == weaponItem.gemSockets[i].itemID).inWorldVersion.GetComponentInChildren<Renderer>();
                    gemRenderer.sharedMaterials = gemStandardMat.sharedMaterials;
                }
                //if (GemSlots[i] != null)
                //{
                //    Destroy(GemSlots[i]);
                //}
                //GameObject gem = Instantiate(weaponItem.gemSockets[i].inWorldVersion, GemSockets[i].transform);
                //EquipedGems[i] = weaponItem.gemSockets[i];
                //GemSlots[i] = gem;
                //PickableItem pickableItem = gem.GetComponent<PickableItem>();
                //pickableItem.DisablePickup();
            }
            else
            {
                if (GemSlots[i] != null)
                {
                    GemSlots[i].SetActive(false);
                }
                //if (GemSlots[i] != null)
                //{
                //    Destroy(GemSlots[i]);
                //}
            }
        }

        if (GetComponent<WeaponEffectsManager>() != null)
        {
            GetComponent<WeaponEffectsManager>().UpdateWeaponFX(weaponItem);
        }

    }
    public void SpawnGemInTrinket(TrinketItem trinketItem)
    {
        for (int i = 0; i < trinketItem.gemSockets.Length; i++)
        {
            if (trinketItem.gemSockets[i] != null)
            {
                if (GemSlots[i] != null)//If its null it should spawn the gem model and then procede with the material assignation
                {
                    GemSlots[i].SetActive(true);
                    Renderer gemRenderer = GemSlots[i].GetComponent<Renderer>();
                    Renderer gemStandardMat = AssetsDatabaseManager._instance.itemsDatabase.GemsDBB.Find(x => x.itemID == trinketItem.gemSockets[i].itemID).inWorldVersion.GetComponentInChildren<Renderer>();
                    gemRenderer.sharedMaterials = gemStandardMat.sharedMaterials;
                }
            }
            else
            {
                if (GemSlots[i] != null)
                {
                    GemSlots[i].SetActive(false);
                }
            }
        }
    }

    public void ManageGemInObject(int objectContext)
    {
        switch (objectContext)
        {
            case 0://Furnace
                FurnaceController furnaceController = GetComponent<FurnaceController>();
                if (furnaceController.gemSocket != null)
                {
                    if (GemSlots[0] != null)//If its null it should spawn the gem model and then procede with the material assignation
                    {
                        GemSlots[0].SetActive(true);
                        Renderer gemRenderer = GemSlots[0].GetComponent<Renderer>();
                        Renderer gemStandardMat = AssetsDatabaseManager._instance.itemsDatabase.GemsDBB.Find(x => x.itemID == furnaceController.gemSocket.itemID).inWorldVersion.GetComponentInChildren<Renderer>();
                        gemRenderer.sharedMaterials = gemStandardMat.sharedMaterials;
                    }
                    //if (GemSlots[0] != null)
                    //{
                    //    Destroy(GemSlots[0]);
                    //}
                    //GameObject gem = Instantiate(furnaceController.gemSocket.inWorldVersion, GemSockets[0].transform);
                    //GemSlots[0] = gem;
                    //PickableItem pickableItem = gem.GetComponent<PickableItem>();
                    //pickableItem.DisablePickup();
                }
                else
                {
                    if (GemSlots[0] != null)
                    {
                        GemSlots[0].SetActive(false);
                    }
                    //if (GemSlots[0] != null)
                    //{
                    //    Destroy(GemSlots[0]);
                    //}
                }
                break;
            default:
                break;
        }
    }

    public List<GemItem> GetEquipedGems()
    {
        List<GemItem> gemsInObject = new List<GemItem>();
        for (int i = 0; i < EquipedGems.Length; i++)
        {
            if (EquipedGems[i] != null)
            {
                gemsInObject.Add(EquipedGems[i]);
            }
        }
        return gemsInObject;
    }

    public void CheckIfItemIsCurrentlyEquiped(WeaponItem weaponItem, PlayerInventory playerInventory)
    {
        if (playerInventory.rightHandWeapon != null)
        {
            if (playerInventory.rightHandWeapon.uniqueItemID == weaponItem.uniqueItemID)
            {
                ManageGemInWeapon(weaponItem);
            }
        }

        if (playerInventory.leftHandWeapon != null)
        {
            if (playerInventory.leftHandWeapon.uniqueItemID == weaponItem.uniqueItemID)
            {
                ManageGemInWeapon(weaponItem);
            }
        }

        if (playerInventory.backSlotWeapon !=null)
        {
            if (playerInventory.backSlotWeapon.uniqueItemID == weaponItem.uniqueItemID)
            {
                ManageGemInWeapon(weaponItem);
            }
        }
    }
}
