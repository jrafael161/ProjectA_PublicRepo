using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;

    public Transform parentOverride;
    public WeaponItem currentWeapon;
    public GameObject currentWeaponModel;
    public bool isLeftHand;
    public bool isRightHand;
    public bool isBackSlot;

    [SerializeField]
    Dictionary<string,GameObject> weaponModelsPool;
    //List<GameObject> weaponModelsPool;

    private void OnEnable()
    {
        if (weaponModelsPool == null)
        {
            //weaponModelsPool = new List<GameObject>();
            weaponModelsPool = new Dictionary<string, GameObject>();
        }
    }
    private void Start()
    {
        weaponSlotManager = GetComponentInParent<WeaponSlotManager>();
    }

    public void UnloadWeapon()//Just disables the model but is still in the player slot so tecnically is still loaded
    {
        if (currentWeaponModel != null && currentWeapon != null)
        {
            currentWeaponModel.gameObject.SetActive(false);
            if (!weaponModelsPool.ContainsKey(currentWeapon.itemName))
            {
                weaponModelsPool.Add(currentWeapon.itemName, currentWeaponModel);
            }
        }
    }

    public void LoadWeaponModel(WeaponItem weaponItem)
    {
        //UnloadWeaponAndDestroy();//Use a pool of objects instead of deleting

        if (currentWeaponModel != null)
        {
            currentWeaponModel.gameObject.SetActive(false);
        }

        if (weaponItem == null)
        {
            weaponItem = weaponSlotManager.unarmedWeapon;
        }

        GameObject model = null;

        if (weaponModelsPool.ContainsKey(weaponItem.itemName))
        {            
            currentWeaponModel = weaponItem.modelPrefab;
            model = currentWeaponModel;
            currentWeaponModel.gameObject.SetActive(true);
        }
        else
        {
            model = Instantiate(weaponItem.modelPrefab);
            weaponItem.modelPrefab = model;
            weaponModelsPool.Add(weaponItem.itemName, model);
            currentWeaponModel = model;
            currentWeaponModel.gameObject.SetActive(true);
        }
        
        if (model != null)
        {
            if (parentOverride != null)
            {
                model.transform.parent = parentOverride;
            }
            else
            {
                model.transform.parent = transform;
            }

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }

        if (weaponItem.isUnarmed == false)
        {
            model.GetComponentInChildren<GemsInObjectsManager>().SpawnGemsInItem(weaponItem);
        }
    }
}
