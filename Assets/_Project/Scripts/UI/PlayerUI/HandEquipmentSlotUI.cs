using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandEquipmentSlotUI : MonoBehaviour
{
    UIManager uIManager;

    public Image icon;
    public WeaponItem weapon;
    public AmmoItem ammo;

    public bool rightHandSlot01;
    public bool rightHandSlot02;
    public bool leftHandSlot01;
    public bool leftHandSlot02;
    public bool ammoSlot;

    public void Start()
    {
        uIManager = GetComponentInParent<UIManager>();
    }
    public void SelectWeaponItem(WeaponItem newWeapon)
    {
        if (newWeapon == null)
            return;

        weapon = newWeapon;
        icon.sprite = weapon.itemIcon;
        icon.enabled = true;
        //gameObject.SetActive(true);
    }

    public void ClearWeaponItem()
    {
        weapon = null;
        icon.sprite = null;
        icon.enabled = false;
        //sgameObject.SetActive(false);
    }

    public void SelectThisSlot()//I dont like how this works
    {
        uIManager.rightHandSlot01Selected = false;
        uIManager.rightHandSlot02Selected = false;
        uIManager.leftHandSlot01Selected = false;
        uIManager.leftHandSlot02Selected = false;
        uIManager.ammoSlotSelected = false;

        if (rightHandSlot01)
        {
            uIManager.rightHandSlot01Selected = true;
        }
        else if(rightHandSlot02)
        {
            uIManager.rightHandSlot02Selected = true;
        }
        else if(leftHandSlot01)
        {
            uIManager.leftHandSlot01Selected = true;
        }
        else if(leftHandSlot02)
        {
            uIManager.leftHandSlot02Selected = true;
        }
        else
        {
            uIManager.ammoSlotSelected = true;
        }
    }

    public void SelectAmmoItem(AmmoItem newSelectedAmmo)
    {
        if (newSelectedAmmo == null)
            return;

        ammo = newSelectedAmmo;
        icon.sprite = newSelectedAmmo.itemIcon;
        icon.enabled = true;
        //gameObject.SetActive(true);
    }

    public void ClearAmmoItem()
    {
        ammo = null;
        icon.sprite = null;
        icon.enabled = false;
        //sgameObject.SetActive(false);
    }

}
