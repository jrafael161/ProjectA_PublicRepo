using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RangedProjectileDamageCollider : MonoBehaviour
{
    public AmmoItem ammoItem;
    bool enemyHit = false;
    bool triedToSpawnArrow = false;

    //private void Start()
    //{
    //    GetComponentInChildren<FLOW.FlowSample>().Simulation = FindObjectOfType<FLOW.FlowSimulation>();//Assigned the moment the arrow is instatiated in combat handler
    //}

    private void OnCollisionEnter(Collision collision)
    {
        enemyHit = false;
        GameObject stuckArrow = null;
        gameObject.SetActive(false);
        ContactPoint contactPoint = collision.GetContact(0);

        stuckArrow = Instantiate(ammoItem.stuckModel, contactPoint.point, Quaternion.Euler(0,0,0));
        
        stuckArrow.transform.parent = collision.transform;
        stuckArrow.transform.rotation = Quaternion.LookRotation(gameObject.transform.forward);

        //Vector3 childScale = stuckArrow.transform.localScale;
        //Vector3 parentScale = collision.transform.localScale;
        //stuckArrow.transform.localScale = new Vector3(childScale.x / parentScale.x, childScale.y / parentScale.y, childScale.z / parentScale.z);

        if (collision.gameObject.tag == "Enemy")
        {
            EnemyStats enemyStats = collision.gameObject.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(ammoItem.damage);
            }
            enemyHit = true;
        }
        else if(triedToSpawnArrow == false)
        {
            if (Random.Range(ammoItem.timesUsed, 100) < 100)
            {
                GameObject itemArrow = Instantiate(ammoItem.inWorldVersion, contactPoint.point, Quaternion.Euler(0, 0, 0));
                itemArrow.transform.parent = collision.transform;
                itemArrow.transform.rotation = Quaternion.LookRotation(gameObject.transform.forward);

                stuckArrow.transform.parent = itemArrow.transform;//Change the parent of the stuck arrow to the pickable arrow, because in case the arrow is picked up quickly(bdeore the stuck arrow is destroyed), it would be left in the struck object if the parent is not changed
                stuckArrow.transform.rotation = Quaternion.LookRotation(itemArrow.transform.forward);

                //childScale = itemArrow.transform.localScale;
                //itemArrow.transform.localScale = new Vector3(childScale.x / parentScale.x, childScale.y / parentScale.y, childScale.z / parentScale.z);

                if (collision.gameObject.GetComponentInChildren<ElementsInteractionHandler>() == null)//If the arrow is water type and upon contact it doesnt react with anything spawn water
                {
                    InstatiateWater();
                }

                PickableItem pickableItem = itemArrow.GetComponent<PickableItem>();
                if (pickableItem != null)
                {
                    pickableItem.item = ammoItem;//Pass the properties of the shoot ammo item to the stuck arrow model
                }

                PrefabGUID prefabGUID = itemArrow.GetComponent<PrefabGUID>();
                if (prefabGUID != null)
                {
                    prefabGUID.uniqueObjectId = pickableItem.item.uniqueItemID;
                }

                AmmoItem stuckArrowItem = pickableItem.item as AmmoItem;
                stuckArrowItem.isStuck = true;
                stuckArrowItem.inWorldVersion = itemArrow;

                Rigidbody itemArrowRigidbody = itemArrow.GetComponent<Rigidbody>();
                itemArrowRigidbody.useGravity = false;
                itemArrowRigidbody.isKinematic = true;

                triedToSpawnArrow = true;
            }
        }

        if (GetComponent<ElementsInteractionHandler>() != null)
        {
            if (GetComponent<WeaponEffectsManager>() != null)
            {
                if (GetComponent<WeaponEffectsManager>().elementLevel != ElementLevel.Level0)
                {
                    WeaponEffectsManager stuckArrowEffectsManager = stuckArrow.GetComponent<WeaponEffectsManager>();

                    stuckArrowEffectsManager.UpdateWeaponFX(GetComponent<WeaponEffectsManager>().elementType, GetComponent<WeaponEffectsManager>().elementLevel);
                    stuckArrowEffectsManager.elementType = GetComponent<WeaponEffectsManager>().elementType;
                    stuckArrowEffectsManager.elementLevel = GetComponent<WeaponEffectsManager>().elementLevel;

                    VisualEffect VFX = stuckArrowEffectsManager.WeaponSlashFX.GetComponentInChildren<VisualEffect>(true);
                    //Color HitColor = stuckArrowEffectsManager.ElementColorTable(stuckArrowEffectsManager.elementType);
                    Color HitColor =  AssetsDatabaseManager._instance.GetElementColor(stuckArrowEffectsManager.elementType);
                    VFX.SetVector4("HitColor", new Vector4(HitColor.r, HitColor.g, HitColor.b, 1));
                }
                else
                {
                    WeaponEffectsManager stuckArrowEffectsManager = stuckArrow.GetComponent<WeaponEffectsManager>();
                    stuckArrowEffectsManager.DisableAllFX();
                }
            }
        }

        if (enemyHit)
        {
            Destroy(stuckArrow, 10);
        }
        else
        {
            Destroy(stuckArrow, 3);
        }

        Destroy(gameObject);
    }

    private void TryToSpawnArrow(Transform collisionPoint)
    {
        if (Random.Range(ammoItem.timesUsed, 100) < 100)
        {
            GameObject itemArrow = Instantiate(ammoItem.inWorldVersion, collisionPoint.position + new Vector3(0,.5f,0), Quaternion.Euler(0, 0, 0));//I add .5 in the y direction so in case the collide happens at ground level it doesnt goes throuh it when the item spawns
            itemArrow.transform.parent = GameObject.Find("Environment").transform;
            itemArrow.transform.rotation = Quaternion.LookRotation(gameObject.transform.forward);

            //childScale = itemArrow.transform.localScale;
            //itemArrow.transform.localScale = new Vector3(childScale.x / parentScale.x, childScale.y / parentScale.y, childScale.z / parentScale.z);

            PickableItem pickableItem = itemArrow.GetComponent<PickableItem>();
            if (pickableItem != null)
            {
                pickableItem.item = ammoItem;//Pass the properties of the shoot ammo item to the stuck arrow model
            }

            PrefabGUID prefabGUID = itemArrow.GetComponent<PrefabGUID>();
            if (prefabGUID != null)
            {
                prefabGUID.uniqueObjectId = pickableItem.item.uniqueItemID;
            }

            AmmoItem stuckArrowItem = pickableItem.item as AmmoItem;
            stuckArrowItem.isStuck = true;
            stuckArrowItem.inWorldVersion = itemArrow;

            //Rigidbody itemArrowRigidbody = itemArrow.GetComponent<Rigidbody>();
            //itemArrowRigidbody.useGravity = false;
            //itemArrowRigidbody.isKinematic = true;

            triedToSpawnArrow = true;
        }
    }

    public void EvaporateWater(Transform collisionPoint)//Can't move to element interaction handler, atm, because the water surface lacks a collider with wich to interact
    {
        if (GetComponent<ElementsInteractionHandler>().GetElementType() == ElementTypes.Fire)
        {
            Debug.Log("Evaporate water");
            GameObject originalSteamPrefab = AssetsDatabaseManager._instance.elementRections[(int)ElementTypes.Steam];
            GameObject instatiatedSteam = Instantiate(originalSteamPrefab, collisionPoint.position, Quaternion.identity, transform.parent);
            //FLOW.FlowModifier waterEvaporator = GetComponentInChildren<FLOW.FlowModifier>(true);
            //waterEvaporator.Mode = FLOW.FlowModifier.ModeType.RemoveFluid;
            //waterEvaporator.transform.parent = transform.parent;
            //waterEvaporator.gameObject.SetActive(true);
            //Destroy(waterEvaporator.gameObject, 1);
            Destroy(instatiatedSteam, 8);

            if (triedToSpawnArrow == false)
            {
                TryToSpawnArrow(collisionPoint);
            }

            switch (GetComponent<ElementsInteractionHandler>().GetElementLevel())
            {
                case ElementLevel.Level0:
                    break;
                case ElementLevel.Level1:
                    //waterEvaporator.Strength *= 5;
                    Destroy(gameObject);
                    break;
                case ElementLevel.Level2:
                    //waterEvaporator.Strength *= 10;
                    Destroy(gameObject);
                    break;
                case ElementLevel.Level3:
                    //waterEvaporator.Strength *= 20;
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    public void InstatiateWater()//Can't move to element interaction handler, atm, because the water surface lacks a collider with wich to interact
    {
        Debug.Log("Add water");
        if (GetComponent<ElementsInteractionHandler>().GetElementType() == ElementTypes.Water)
        {
            //FLOW.FlowModifier waterCreator = GetComponentInChildren<FLOW.FlowModifier>(true);
            //waterCreator.transform.parent = transform.parent;
            //waterCreator.gameObject.SetActive(true);

            //Destroy(waterCreator.gameObject, 1);

            switch (GetComponent<ElementsInteractionHandler>().GetElementLevel())
            {
                case ElementLevel.Level0:
                    break;
                case ElementLevel.Level1:
                    //waterCreator.Strength *= 1;
                    break;
                case ElementLevel.Level2:
                    //waterCreator.Strength *= 3;
                    break;
                case ElementLevel.Level3:
                    //waterCreator.Strength *= 6;
                    break;
                default:
                    break;
            }
        }
    }
}
